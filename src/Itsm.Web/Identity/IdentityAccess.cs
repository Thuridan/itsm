using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
namespace Itsm.Web.Identity;

public sealed class IdentityAccess(IdentityDb db, IdentitySettings settings)
{
    public static (Guid Tenant, Guid Object)? ReadIdentity(ClaimsPrincipal principal, string tenant)
    {
        if (principal.Identity?.IsAuthenticated != true || !Guid.TryParse(tenant, out var expected) ||
            !Guid.TryParse(principal.FindFirstValue("tid"), out var tid) || tid != expected ||
            !Guid.TryParse(principal.FindFirstValue("oid"), out var oid) || oid == Guid.Empty) return null;
        return (tid, oid);
    }
    public async Task<LocalUser?> FindAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var key = ReadIdentity(principal, settings.TenantId);
        return key is null ? null : await db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.TenantId == key.Value.Tenant && u.ObjectId == key.Value.Object, ct);
    }
    public async Task<LocalUser> RegisterAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var key = ReadIdentity(principal, settings.TenantId) ?? throw new InvalidOperationException("Invalid directory identity.");
        var found = await FindAsync(principal, ct);
        if (found is not null) return found;
        var name = principal.FindFirstValue("name") ?? "Colaborador";
        var user = new LocalUser { Id = Guid.NewGuid(), TenantId = key.Tenant, ObjectId = key.Object,
            DisplayName = name[..Math.Min(200, name.Length)], State = "Pending", CreatedAt = DateTimeOffset.UtcNow };
        db.Users.Add(user);
        db.Audit.Add(new IdentityAudit { ActorId = user.Id, TargetId = user.Id, Action = "PendingRegistration", At = DateTimeOffset.UtcNow });
        try { await db.SaveChangesAsync(ct); return user; }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
        {
            db.ChangeTracker.Clear();
            return await FindAsync(principal, ct) ?? throw new InvalidOperationException("Registration conflict.");
        }
    }
    public async Task<bool> IsActiveAsync(LocalUser? user, CancellationToken ct) =>
        user is { State: "Active", UnitId: not null, AreaId: not null } &&
        await db.Units.AnyAsync(u => u.Id == user.UnitId && u.Active, ct) &&
        await db.Areas.AnyAsync(a => a.Id == user.AreaId && a.Active, ct);
    public async Task<bool> IsAdministratorAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        var user = await FindAsync(principal, ct);
        return user?.IsAdministrator == true && await IsActiveAsync(user, ct);
    }
}
