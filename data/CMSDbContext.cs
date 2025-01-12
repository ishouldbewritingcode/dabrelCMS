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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CMSSite>().HasData(
				new CMSSite
				{
					SiteId = 1,
					Name = "test",
					Design = "superbee",
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
					SiteUrlId = 1,
					SiteId = 1,
					Url = "localhost",
					Primary = true
				});

			modelBuilder.Entity<CMSUser>().HasData(
				new CMSUser
				{
					Provider = "Cookies",
					NameIdentifier = "",
					UserId = 1,
					SiteId = 1,
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
					PageId = 1,
					ParentId = 0,
					Sort = 1,
					SiteId = 1,
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