var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
var app = builder.Build();
app.UseExceptionHandler();
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "same-origin";
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; img-src 'self' data:; style-src 'self'; script-src 'self'; connect-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
    if (context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/health"))
        context.Response.Headers.CacheControl = "no-store";
    await next();
});
// Public shell only. No business data or development impersonation endpoints exist.
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }));
// A running HTTP process does not mean the ITSM is ready to receive real users.
app.MapGet("/health/ready", () => Results.Json(new { status = "not_ready" }, statusCode: 503));
app.MapGet("/api/v1/system", () => Results.Ok(new
{
    product = "Arbos — Central de Serviços",
    stage = "foundation",
    authenticationEnabled = false,
    persistenceEnabled = false,
    acceptingTickets = false
}));
app.Map("/api/{**path}", () => Results.NotFound());
app.Map("/health/{**path}", () => Results.NotFound());
app.MapFallbackToFile("index.html");
app.Run();
