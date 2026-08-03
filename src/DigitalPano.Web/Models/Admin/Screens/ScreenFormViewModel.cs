using System.ComponentModel.DataAnnotations;

namespace DigitalPano.Web.Models.Admin.Screens;

public sealed class ScreenFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ekran adı zorunludur.")]
    [StringLength(100, ErrorMessage = "Ekran adı en fazla 100 karakter olabilir.")]
    [Display(Name = "Ekran adı")]
    public string Name { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "Adres anahtarı en fazla 120 karakter olabilir.")]
    [RegularExpression("^[a-z0-9-]*$", ErrorMessage = "Adres anahtarında yalnızca küçük harf, rakam ve tire kullanılabilir.")]
    [Display(Name = "Adres anahtarı (slug)")]
    public string? Slug { get; set; }

    [StringLength(200, ErrorMessage = "Konum en fazla 200 karakter olabilir.")]
    [Display(Name = "Fiziksel konum")]
    public string? Location { get; set; }

    [Display(Name = "Ekran aktif")]
    public bool IsActive { get; set; } = true;
}
