using dabrelCMS.models;
using System.Runtime.ExceptionServices;

namespace dabrelCMS.data
{
	public class CMSDbContext : DbContext
	{
		public DbSet<CMSUser> CMSUsers { get; set; }
		public DbSet<CMSSite> CMSSites { get; set; }
		public DbSet<CMSSiteUrl> CMSSiteUrls { get; set; }
		public DbSet<CMSPage> CMSPages { get; set; }
		public DbSet<CMSPageBlock> CMSPageBlocks { get; set; }
		public DbSet<CMSBlock> CMSBlocks { get; set; }
		public DbSet<CMSItem> CMSItems { get; set; }
		public DbSet<CMSFile> CMSFiles { get; set; }

		public CMSDbContext() : base()
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		=> options.UseSqlite(CMSConfig.ConStr);

		private static readonly Guid _seedSiteId    = new Guid("00000000-0000-7000-8000-000000000001");
		private static readonly Guid _seedSiteUrlId = new Guid("00000000-0000-7000-8000-000000000002");
		private static readonly Guid _seedUserId    = new Guid("00000000-0000-7000-8000-000000000003");
		private static readonly Guid _seedPageId    = new Guid("00000000-0000-7000-8000-000000000004");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CMSSite>().HasData(
				new CMSSite
				{
					SiteId = _seedSiteId,
					Name = "test",
					Design = "mountain",
					Title = "title",
					SubTitle = "subtitle",
					Footer1 = "footer 1",
					Footer2 = "",
					Footer3 = "",
					Footer4 = "",
					MetaDescription = "Description",
					MetaImagePath = "",
					OnAllPages = "",
					BodyTop = "",
					BodyBottom = "",
					ImageFileName = "",
					Created = DateTime.Parse("1/1/2025 12:01am"),
					FaviconUrl = ""
				});

			modelBuilder.Entity<CMSSiteUrl>().HasData(
				new CMSSiteUrl
				{
					SiteUrlId = _seedSiteUrlId,
					SiteId = _seedSiteId,
					Url = "localhost",
					Primary = true
				});

			modelBuilder.Entity<CMSUser>().HasData(
				new CMSUser
				{
					Provider = "Cookies",
					NameIdentifier = "",
					UserId = _seedUserId,
					SiteId = _seedSiteId,
					Email = "test@dabrel.com",
					Password = "test",
					FirstName = "test",
					LastName = "user",
					Mobile = "",
					Roles = "admin"
				});

			modelBuilder.Entity<CMSPage>().HasData(
				new CMSPage
				{
					PageId = _seedPageId,
					ParentId = null,
					Sort = 1,
					SiteId = _seedSiteId,
					isOn = true,
					isPrivate = false,
					isHidden = false,
					Shortcut = "",
					Tags = "home",
					NavTitle = "Home",
					Title = "Welcome",
					Summary = "Page summary goes here",
					HeroImage = ""
				});
		}
	}
}