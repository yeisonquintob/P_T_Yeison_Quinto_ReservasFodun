namespace ReservasFodun.Web.ViewModels.Admin;

public class AdminUsuarioDetalleViewModel
{
    public string Id { get; set; } = string.Empty;

    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }

    public int AccessFailedCount { get; set; }

    public IList<string> Roles { get; set; } = new List<string>();

    public bool EstaBloqueado =>
        LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;
}
