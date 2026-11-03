using Itsm.Web.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
var settings=builder.Configuration.GetSection("Identity").Get<IdentitySettings>() ?? new();
var connection=builder.Configuration.GetConnectionString("Identity");
settings.Validate(connection);
builder.Services.AddSingleton(settings);
builder.Services.AddAuthorization();
var auth=builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
auth.AddCookie(options => {
    options.Cookie.Name="__Host-Arbos.Session";
    options.Cookie.HttpOnly=true; options.Cookie.SecurePolicy=CookieSecurePolicy.Always;
    options.Cookie.SameSite=SameSiteMode.Lax;
    options.ExpireTimeSpan=TimeSpan.FromHours(8); options.SlidingExpiration=false;
    options.Events.OnRedirectToLogin=ctx=>{ctx.Response.StatusCode=401;return Task.CompletedTask;};
    options.Events.OnRedirectToAccessDenied=ctx=>{ctx.Response.StatusCode=403;return Task.CompletedTask;};
});
builder.Services.AddAntiforgery(options=>{
    options.HeaderName="X-CSRF-TOKEN"; options.Cookie.Name="__Host-Arbos.Csrf";
    options.Cookie.SecurePolicy=CookieSecurePolicy.Always; options.Cookie.HttpOnly=true; options.Cookie.SameSite=SameSiteMode.Strict;
});
if(settings.Enabled)
{
    builder.Services.AddDataProtection().SetApplicationName("Arbos.ITSM."+builder.Environment.EnvironmentName)
        .PersistKeysToFileSystem(new DirectoryInfo(settings.KeyPath));
    builder.Services.AddDbContext<IdentityDb>(options=>options.UseSqlServer(connection));
    builder.Services.AddScoped<IdentityAccess>();
    auth.AddOpenIdConnect(options=>{
        options.Authority=$"https://login.microsoftonline.com/{Guid.Parse(settings.TenantId)}/v2.0";
        options.ClientId=settings.ClientId; options.ClientSecret=settings.ClientSecret;
        options.ResponseType="code"; options.UsePkce=true; options.SaveTokens=false;
        options.MapInboundClaims=false; options.RequireHttpsMetadata=true;
        options.CallbackPath="/signin-oidc";
        options.Scope.Clear(); options.Scope.Add("openid"); options.Scope.Add("profile");
        options.TokenValidationParameters=new TokenValidationParameters {ValidateIssuer=true,ValidateAudience=true,NameClaimType="name"};
        options.Events.OnRedirectToIdentityProvider=ctx=>{
            ctx.ProtocolMessage.RedirectUri=settings.PublicOrigin.TrimEnd('/')+"/signin-oidc";
            return Task.CompletedTask;
        };
        options.Events.OnTokenValidated=async ctx=>{
            var access=ctx.HttpContext.RequestServices.GetRequiredService<IdentityAccess>();
            if(ctx.Principal is null || IdentityAccess.ReadIdentity(ctx.Principal,settings.TenantId) is null) {ctx.Fail("Invalid tenant identity.");return;}
            try {
                var user=await access.RegisterAsync(ctx.Principal,ctx.HttpContext.RequestAborted);
                if(user.State=="Disabled")ctx.Fail("Account unavailable.");
            } catch { ctx.Fail("Account unavailable."); }
        };
        options.Events.OnRemoteFailure=ctx=>{ctx.HandleResponse();ctx.Response.Redirect("/?authError=1");return Task.CompletedTask;};
    });
}
var app=builder.Build();
app.UseExceptionHandler();
app.Use(async (context,next)=>{
    context.Response.Headers["X-Content-Type-Options"]="nosniff";
    context.Response.Headers["Referrer-Policy"]="same-origin";
    context.Response.Headers["Content-Security-Policy"]="default-src 'self'; img-src 'self' data:; style-src 'self'; script-src 'self'; connect-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
    if(context.Request.Path.StartsWithSegments("/api")||context.Request.Path.StartsWithSegments("/health")||context.Request.Path.StartsWithSegments("/auth"))context.Response.Headers.CacheControl="no-store";
    await next();
});
app.UseDefaultFiles();app.UseStaticFiles();app.UseAuthentication();app.UseAuthorization();
app.MapGet("/health/live",()=>Results.Ok(new {status="alive"}));
// Ticket service is still not ready; identity configuration alone never changes this status.
app.MapGet("/health/ready",()=>Results.Json(new {status="not_ready"},statusCode:503));
app.MapGet("/api/v1/system",()=>Results.Ok(new {product="Arbos — Central de Serviços",stage="identity",authenticationEnabled=settings.Enabled,persistenceEnabled=settings.Enabled,acceptingTickets=false}));
app.MapIdentity(settings);
app.Map("/api/{**path}",()=>Results.NotFound());
app.Map("/auth/{**path}",()=>Results.NotFound());
app.Map("/health/{**path}",()=>Results.NotFound());
app.MapFallbackToFile("index.html");app.Run();
