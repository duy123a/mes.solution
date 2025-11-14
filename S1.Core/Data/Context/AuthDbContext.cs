using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using S1.Core.Entities;

namespace S1.Core.Data.Context
{
    public class AuthDbContext : IdentityDbContext<AppUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("auth");

            builder.UseOpenIddict();
        }
    }
}
