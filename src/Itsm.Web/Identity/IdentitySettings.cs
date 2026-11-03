namespace Itsm.Web.Identity;

public sealed class IdentitySettings
{
    public bool Enabled { get; init; }
    public string TenantId { get; init; } = "";
    public string ClientId { get; init; } = "";
    public string ClientSecret { get; init; } = "";
    public string PublicOrigin { get; init; } = "";
    public string KeyPath { get; init; } = "";
    public void Validate(string? connectionString)
    {
        if (!Enabled) return;
        if (!Guid.TryParse(TenantId, out var tenant) || tenant == Guid.Empty ||
            !Guid.TryParse(ClientId, out var client) || client == Guid.Empty || string.IsNullOrWhiteSpace(ClientSecret))
            throw new InvalidOperationException("Identity requires tenant, client and a credential.");
        if (!Uri.TryCreate(PublicOrigin, UriKind.Absolute, out var uri) || uri.Scheme != "https" ||
            uri.AbsolutePath != "/" || uri.Query != "" || uri.Fragment != "" || uri.UserInfo != "")
            throw new InvalidOperationException("Identity PublicOrigin must be an HTTPS origin.");
        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(KeyPath) || !Path.IsPathRooted(KeyPath))
            throw new InvalidOperationException("Identity requires SQL Server and an absolute persistent key path.");
    }
}
