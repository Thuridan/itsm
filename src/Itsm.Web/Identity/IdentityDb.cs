using Microsoft.EntityFrameworkCore;
namespace Itsm.Web.Identity;

public sealed class DirectoryUnit { public Guid Id { get; set; } public string Name { get; set; } = ""; public bool Active { get; set; } = true; }
public sealed class DirectoryArea { public Guid Id { get; set; } public string Name { get; set; } = ""; public bool Active { get; set; } = true; }
public sealed class LocalUser
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ObjectId { get; set; }
    public string DisplayName { get; set; } = "";
    public string State { get; set; } = "Pending";
    public bool IsAdministrator { get; set; }
    public Guid? UnitId { get; set; }
    public Guid? AreaId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public byte[] Version { get; set; } = [];
}
public sealed class IdentityAudit
{
    public long Id { get; set; }
    public Guid ActorId { get; set; }
    public Guid TargetId { get; set; }
    public string Action { get; set; } = "";
    public DateTimeOffset At { get; set; }
}
public sealed class IdentityDb(DbContextOptions<IdentityDb> options) : DbContext(options)
{
    public DbSet<LocalUser> Users => Set<LocalUser>();
    public DbSet<DirectoryUnit> Units => Set<DirectoryUnit>();
    public DbSet<DirectoryArea> Areas => Set<DirectoryArea>();
    public DbSet<IdentityAudit> Audit => Set<IdentityAudit>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("identity");
        b.Entity<LocalUser>(e => {
            e.ToTable("Users", t => t.HasCheckConstraint("CK_Users_State", "[State] IN ('Pending','Active','Disabled')"));
            e.HasKey(x => x.Id); e.HasIndex(x => new {x.TenantId, x.ObjectId}).IsUnique();
            e.Property(x => x.DisplayName).HasMaxLength(200); e.Property(x => x.State).HasMaxLength(20);
            e.Property(x => x.Version).IsRowVersion();
            e.HasOne<DirectoryUnit>().WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne<DirectoryArea>().WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
        });
        b.Entity<DirectoryUnit>(e => {e.ToTable("Units"); e.HasKey(x=>x.Id); e.Property(x=>x.Name).HasMaxLength(150);});
        b.Entity<DirectoryArea>(e => {e.ToTable("Areas"); e.HasKey(x=>x.Id); e.Property(x=>x.Name).HasMaxLength(150);});
        b.Entity<IdentityAudit>(e => {e.ToTable("Audit"); e.HasKey(x=>x.Id); e.Property(x=>x.Action).HasMaxLength(100);});
    }
}
