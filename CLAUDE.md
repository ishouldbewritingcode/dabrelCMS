# dabrelCMS

A custom ASP.NET Core 10.0 CMS with multi-site support, HTMX-driven frontend, and SQLite storage.

## Build & Run

```bash
dotnet build
dotnet run
```

EF Core migrations:
```bash
dotnet ef migrations add <Name>
dotnet ef database update
```

## Architecture

### Request Pipeline

All web traffic flows through two custom middleware classes in order:

1. **`CMSUploaderMiddleware`** — handles file uploads at `/adminupload`, passes everything else through
2. **`CMSMiddleware`** — calls `Site.GetSite(context)` and writes the returned HTML string to the response

There are no controllers or Razor Pages. Every request is handled by `Site.GetSite()`, which switches on the normalized path and returns a complete HTML string.

### Non-DI Pattern

Business logic classes (`Site`, `Admin`, `SiteManager`, `Uploader`, etc.) are **not registered in DI** — they are instantiated with `new`. This is intentional.

- Do NOT add constructor injection to these classes
- For logging, use `Serilog.Log.ForContext<T>()` as a `private static readonly` field
- For `IMemoryCache`, resolve via `context.RequestServices.GetRequiredService<IMemoryCache>()`

```csharp
private static readonly Serilog.ILogger _log = Serilog.Log.ForContext<MyClass>();
```

The middleware classes (`CMSMiddleware`, `CMSUploaderMiddleware`) DO use DI constructor injection since ASP.NET Core manages them.

### Path Routing

`Site.GetSite()` normalizes the request path and switches on it:

| Path | Handler |
|------|---------|
| `admin/*` | `Admin.GetPage()` (requires auth) |
| `sitemanager/*` | `SiteManager.GetPage()` (requires sitemanager role) |
| `auth` | Login / TOTP flow |
| `search` | Full-text search across pages and items |
| anything else | Public page lookup by shortcut |

### Designs (Templating)

Pages are rendered by loading an HTML template from `wwwroot/designs/{design}/{design}.htm` and doing string replacements on `{{placeholder}}` tokens. Leftover `{{...}}` tokens are stripped by regex before returning.

Available designs: `superbee`, `theblues`, `mountain`, `icanfixit`
Admin UI design lives in `wwwroot/designs/admin/`

### Multi-Site

The CMS is domain-aware. Each request resolves its site by looking up `CMSSiteUrls` by hostname. All content (pages, blocks, items, users) belongs to a `SiteId`.

## Data Model

```
CMSSite
  └── CMSSiteUrl (one per hostname)
  └── CMSPage
        └── CMSPageBlock (join table, ordered by Sort)
              └── CMSBlock
                    └── CMSItem (sorted by Start desc, top 5 shown)
  └── CMSUser (has Salt/Password hash, optional TotpSecret, RoleList)
  └── CMSFile
```

`CMSDbContext` is in `App_Data/CMSDbContext.cs`. It is always instantiated directly:
```csharp
using CMSDbContext dbcontext = new CMSDbContext();
```

## Caching

Public pages are cached in `IMemoryCache` inside `Site.GetSite()`, keyed by `page:{domain}:{path}:{full|htmx}`. Full-page and HTMX partial responses are cached separately.

**Not cached:** admin, sitemanager, auth, search, private pages, non-GET requests.

`CMSCache` (root namespace) manages per-domain `CancellationTokenSource` objects. Call `CMSCache.InvalidateSite(_domain)` after any content-modifying DB save to evict all cached pages for that domain at once. This is already wired into `Admin.cs` for all save operations.

## Authentication

- Cookie-based JWT (`token` cookie), validated per request in `Site.GetSite()`
- Optional TOTP (two-factor) via `totp_pending` cookie intermediate step
- `TotpReplayCache` prevents code reuse within the TOTP window
- Admin access requires a valid JWT; sitemanager access additionally requires `"sitemanager"` in `authUser.RoleList`

## Logging

Serilog is configured via `appsettings.json`. In Development, logs go to both Console and `App_Logs/log-{date}.txt`. In Production, file only.

Use structured logging with message templates — not string interpolation:
```csharp
_log.Information("Page saved: {PageId} {Shortcut}", page.PageId, page.Shortcut); // correct
_log.Information($"Page saved: {page.PageId}");  // avoid — loses structured data
```

## Key Files

| File | Purpose |
|------|---------|
| `Program.cs` | App startup, Serilog config, service registration |
| `CMSMiddleware.cs` | Main request handler — calls `Site.GetSite()` |
| `CMSUploaderMiddleware.cs` | File upload handler |
| `CMSCache.cs` | Per-domain cache invalidation token management |
| `code/Site.cs` | Core request router — public pages, auth, search |
| `code/Admin.cs` | Admin CRUD — pages, blocks, items, files, site settings |
| `code/SiteManager.cs` | Site manager UI |
| `code/Uploader.cs` | File upload processing |
| `code/JwtUtils.cs` | JWT generation and validation |
| `code/Common.cs` | Shared helpers (404, login page, file reading) |
| `App_Data/CMSDbContext.cs` | EF Core DbContext |
| `appsettings.json` | Connection string, JWT key, Serilog (file sink) |
| `appsettings.Development.json` | Serilog overrides (adds console sink) |
