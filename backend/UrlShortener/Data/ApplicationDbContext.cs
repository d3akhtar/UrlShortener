using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Model;

namespace UrlShortener.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UrlCode> UrlCodes { get; set; }
        public DbSet<Alias> Aliases { get; set; }
        public DbSet<UrlCodeKey> UrlCodeKeys { get; set; }
        //public DbSet<ApplicationUser> ApplicationUsers { get; set; } maybe add if i ever feel like adding more properties to application user
        public DbSet<ApplicationUserUrlCode> ApplicationUserUrlCodes { get; set; }
        public DbSet<ApplicationUserAlias> ApplicationUserAliases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region AutoInclude
            modelBuilder.Entity<ApplicationUserUrlCode>().Navigation(e => e.UrlCode).AutoInclude();
            modelBuilder.Entity<ApplicationUserAlias>().Navigation(e => e.Alias).AutoInclude();
            #endregion
        }
    }
}
