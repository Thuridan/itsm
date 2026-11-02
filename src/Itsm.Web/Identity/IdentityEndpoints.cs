using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
namespace Itsm.Web.Identity;

public sealed record ActivateRequest(Guid UnitId, Guid AreaId, string Version);
public static class IdentityEndpoints
{
    public static void MapIdentity(this WebApplication app, IdentitySettings settings)
    {
        if (!settings.Enabled)
        {
            app.MapGet("/auth/login", () => Results.Problem(statusCode:503, title:"Login não configurado"));
            app.MapGet("/api/v1/me", () => Results.Ok(new {authenticated=false}));
            app.MapPost("/auth/logout", () => Results.StatusCode(503));
            return;
        }
        app.MapGet("/auth/login", () => Results.Challenge(new AuthenticationProperties { RedirectUri = "/" }, [OpenIdConnectDefaults.AuthenticationScheme]));
        app.MapGet("/api/v1/me", async Task<IResult> (HttpContext ctx, IdentityAccess access, CancellationToken ct) => {
            if (ctx.User.Identity?.IsAuthenticated != true) return Results.Ok(new { authenticated=false });
            var user = await access.FindAsync(ctx.User, ct);
            if (user is null) return Results.Unauthorized();
            var active = await access.IsActiveAsync(user, ct);
            return Results.Ok(new { authenticated=true, displayName=user.DisplayName, state=user.State,
                accessGranted=active, isAdministrator=active && user.IsAdministrator, unitId=user.UnitId, areaId=user.AreaId });
        });
        app.MapGet("/api/v1/antiforgery", (HttpContext ctx, IAntiforgery csrf) => Results.Ok(new {token=csrf.GetAndStoreTokens(ctx).RequestToken})).RequireAuthorization();
        app.MapPost("/auth/logout", async Task<IResult> (HttpContext ctx, IAntiforgery csrf) => {
            try { await csrf.ValidateRequestAsync(ctx); } catch (AntiforgeryValidationException) { return Results.BadRequest(); }
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.NoContent();
        }).RequireAuthorization();
        var admin = app.MapGroup("/api/v1/admin").RequireAuthorization();
        admin.AddEndpointFilter(async (context, next) => {
            var http=context.HttpContext;
            var access=http.RequestServices.GetRequiredService<IdentityAccess>();
            if (!await access.IsAdministratorAsync(http.User, http.RequestAborted)) return Results.Forbid();
            if (!HttpMethods.IsGet(http.Request.Method)) {
                try { await http.RequestServices.GetRequiredService<IAntiforgery>().ValidateRequestAsync(http); }
                catch (AntiforgeryValidationException) { return Results.BadRequest(); }
            }
            return await next(context);
        });
        admin.MapGet("/directory", async (IdentityDb db, CancellationToken ct) => Results.Ok(new {
            units = await db.Units.Where(x=>x.Active).OrderBy(x=>x.Name).Select(x=>new{x.Id,x.Name}).ToListAsync(ct),
            areas = await db.Areas.Where(x=>x.Active).OrderBy(x=>x.Name).Select(x=>new{x.Id,x.Name}).ToListAsync(ct)
        }));
        admin.MapGet("/users", async (IdentityDb db, IdentitySettings config, int? page, CancellationToken ct) => {
            var index=Math.Clamp(page ?? 1,1,100000);
            return Results.Ok(await db.Users.AsNoTracking().Where(u=>u.TenantId==Guid.Parse(config.TenantId)).OrderBy(u=>u.CreatedAt).ThenBy(u=>u.Id).Skip((index-1)*50).Take(50)
                .Select(u=>new {u.Id,u.DisplayName,u.State,u.UnitId,u.AreaId,version=Convert.ToBase64String(u.Version)}).ToListAsync(ct));
        });
        admin.MapPost("/users/{id:guid}/activate", async Task<IResult> (Guid id, ActivateRequest request, HttpContext ctx, IdentityDb db, IdentityAccess access, IdentitySettings config, CancellationToken ct) => {
            byte[] version;
            try { version=Convert.FromBase64String(request.Version); } catch (Exception ex) when (ex is FormatException or ArgumentNullException) { return Results.BadRequest(); }
            if (version.Length!=8) return Results.BadRequest();
            var user=await db.Users.SingleOrDefaultAsync(x=>x.Id==id && x.TenantId==Guid.Parse(config.TenantId),ct);
            if (user is null) return Results.NotFound();
            if (user.State!="Pending") return Results.Conflict(new {message="O cadastro já foi tratado."});
            if (!await db.Units.AnyAsync(x=>x.Id==request.UnitId && x.Active,ct) || !await db.Areas.AnyAsync(x=>x.Id==request.AreaId && x.Active,ct))
                return Results.BadRequest(new {message="Área e unidade ativas são obrigatórias."});
            db.Entry(user).Property(x=>x.Version).OriginalValue=version;
            user.UnitId=request.UnitId; user.AreaId=request.AreaId; user.State="Active";
            // This operation can only activate a collaborator. It never grants administrator rights.
            user.IsAdministrator=false;
            var actor=await access.FindAsync(ctx.User,ct);
            db.Audit.Add(new IdentityAudit { ActorId=actor!.Id,TargetId=id,Action="CollaboratorActivated",At=DateTimeOffset.UtcNow });
            try { await db.SaveChangesAsync(ct); } catch (DbUpdateConcurrencyException) { return Results.Conflict(); }
            return Results.NoContent();
        });
    }
}
