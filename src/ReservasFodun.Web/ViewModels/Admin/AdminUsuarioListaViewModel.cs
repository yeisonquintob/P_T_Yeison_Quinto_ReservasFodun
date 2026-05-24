namespace ReservasFodun.Web.ViewModels.Admin;

public class AdminUsuarioListaViewModel
{
    public string Id { get; set; } = string.Empty;

    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }

    public bool EstaBloqueado =>
        LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;
}
