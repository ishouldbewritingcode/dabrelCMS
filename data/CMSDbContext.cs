using dabrelCMS.models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ExceptionServices;

namespace dabrelCMS.data
{
	public class CMSDbContext:DbContext
	{
		public DbSet<CMSUser> CMSUsers { get; set; }

		public CMSDbContext(DbContextOptions<CMSDbContext> options) : base(options) 
		{ 
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<CMSUser>( entity =>
			{
				entity.HasKey(e => e.UserId);
				entity.Property(e => e.UserId);
				entity.Property(e => e.Provider).HasMaxLength(250);
				entity.Property(e => e.NameIdentifier).HasMaxLength(500);
				entity.Property(e => e.UserName).HasMaxLength(250);
				entity.Property(e => e.Password).HasMaxLength(250);
				entity.Property(e => e.Email).HasMaxLength(250);
				entity.Property(e => e.EmailConfirmed).HasMaxLength(250);
				entity.Property(e => e.FirstName).HasMaxLength(250);
				entity.Property(e => e.LastName).HasMaxLength(250);
				entity.Property(e => e.Mobile).HasMaxLength(250);
				entity.Property(e => e.Roles).HasMaxLength(1000);

				entity.HasData(new CMSUser
				{
					Provider = "Cookies",
					NameIdentifier = "",
					UserId = 1,
					Email = "junk@dabrel.com",
					EmailConfirmed = "confirmed",
					UserName = "junk@dabrel.com",
					Password = "junk",
					FirstName = "junk",
					LastName = "user",
					Mobile = "",
					Roles = "admin"
				});
			});
		}

	}
}
