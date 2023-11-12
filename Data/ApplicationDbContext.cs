using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Munkanaplo2.Models;

namespace Munkanaplo2.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

/*protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<ProjectMembership>()
        .HasKey(pm => pm.ProjectMembershipId);

    modelBuilder.Entity<ProjectMembership>()
        .HasOne(pm => pm.Project)
        .WithMany(p => p.ProjectMembers)
        .HasForeignKey(pm => pm.ProjectId);

    base.OnModelCreating(modelBuilder);
}*/
    public DbSet<Munkanaplo2.Models.JobModel> JobModel { get; set; } = default!;
    public DbSet<Munkanaplo2.Models.ProjectModel> ProjectModel { get; set; } = default!;
    public DbSet<ProjectMembership> ProjectMemberships { get; internal set; }

    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectMembership>()
                .HasKey(pm => new { pm.ProjectId, pm.Member });

            modelBuilder.Entity<ProjectMembership>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.ProjectMembers)
                .HasForeignKey(pm => pm.ProjectId);

            base.OnModelCreating(modelBuilder);
        }*/
}

