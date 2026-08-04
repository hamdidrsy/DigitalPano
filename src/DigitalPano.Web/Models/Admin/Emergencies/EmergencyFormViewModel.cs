using System.ComponentModel.DataAnnotations;
using DigitalPano.Web.Models.Admin.Announcements;

namespace DigitalPano.Web.Models.Admin.Emergencies;

public sealed class EmergencyFormViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200)]
    [Display(Name = "Acil duyuru başlığı")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(4000)]
    [Display(Name = "Açıklama")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "İsteğe bağlı görsel")]
    public int? MediaId { get; set; }

    [Range(1, 1440, ErrorMessage = "Süre 1 ile 1440 dakika arasında olmalıdır.")]
    [Display(Name = "En fazla yayın süresi (dakika)")]
    public int DurationMinutes { get; set; } = 60;

    [Display(Name = "Hedef ekranlar")]
    public List<int> SelectedScreenIds { get; set; } = [];

    [Range(typeof(bool), "true", "true", ErrorMessage = "Acil yayını başlatmak için onay vermelisiniz.")]
    [Display(Name = "Bu duyurunun seçili ekranları tamamen kaplayacağını onaylıyorum")]
    public bool IsConfirmed { get; set; }

    public IReadOnlyList<ScreenOptionViewModel> Screens { get; set; } = [];
    public IReadOnlyList<MediaOptionViewModel> ImageOptions { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SelectedScreenIds.Count == 0)
        {
            yield return new ValidationResult("En az bir ekran seçilmelidir.", [nameof(SelectedScreenIds)]);
        }
    }
}
