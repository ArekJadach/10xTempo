using _10xTempo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _10xTempo.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();
    public DbSet<Report> Reports => Set<Report>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Report>()
            .Property(r => r.Hours)
            .HasPrecision(18, 2);

        builder.Entity<UserCompany>()
            .HasIndex(x => new { x.UserId, x.CompanyId })
            .IsUnique();
    }
}