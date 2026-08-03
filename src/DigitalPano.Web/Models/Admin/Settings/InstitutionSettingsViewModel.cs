using System.ComponentModel.DataAnnotations;

namespace DigitalPano.Web.Models.Admin.Settings;

public sealed class InstitutionSettingsViewModel
{
    [Required(ErrorMessage = "Kurum adı zorunludur.")]
    [StringLength(200, ErrorMessage = "Kurum adı en fazla 200 karakter olabilir.")]
    [Display(Name = "Kurum adı")]
    public string InstitutionName { get; set; } = string.Empty;

    [Display(Name = "Kurum logosu")]
    public int? LogoMediaId { get; set; }

    [Required(ErrorMessage = "Ana renk zorunludur.")]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Renk #RRGGBB biçiminde olmalıdır.")]
    [Display(Name = "Ana renk")]
    public string PrimaryColor { get; set; } = "#0D6EFD";

    [Required(ErrorMessage = "İkincil renk zorunludur.")]
    [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Renk #RRGGBB biçiminde olmalıdır.")]
    [Display(Name = "İkincil renk")]
    public string SecondaryColor { get; set; } = "#6C757D";

    [Required(ErrorMessage = "Şehir zorunludur.")]
    [StringLength(100, ErrorMessage = "Şehir en fazla 100 karakter olabilir.")]
    [Display(Name = "Şehir")]
    public string City { get; set; } = "İstanbul";

    public IReadOnlyList<LogoOptionViewModel> LogoOptions { get; set; } = [];
}

public sealed record LogoOptionViewModel(int Id, string OriginalFileName);
