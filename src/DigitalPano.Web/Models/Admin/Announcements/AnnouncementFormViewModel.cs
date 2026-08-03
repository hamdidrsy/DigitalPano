using System.ComponentModel.DataAnnotations;

namespace DigitalPano.Web.Models.Admin.Announcements;

public sealed class AnnouncementFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    [Display(Name = "Başlık")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(4000, ErrorMessage = "Açıklama en fazla 4000 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
    [Display(Name = "Yayın başlangıcı")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
    [Display(Name = "Yayın bitişi")]
    public DateTime EndDate { get; set; }

    [Range(1, 3600, ErrorMessage = "Gösterim süresi 1 ile 3600 saniye arasında olmalıdır.")]
    [Display(Name = "Gösterim süresi (saniye)")]
    public int DisplayDurationSeconds { get; set; } = 10;

    [Range(-10000, 10000, ErrorMessage = "Sıra -10000 ile 10000 arasında olmalıdır.")]
    [Display(Name = "Yayın sırası")]
    public int SortOrder { get; set; }

    [Display(Name = "Yayına açık")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Gösterilecek ekranlar")]
    public List<int> SelectedScreenIds { get; set; } = [];

    public IReadOnlyList<ScreenOptionViewModel> Screens { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
        {
            yield return new ValidationResult(
                "Bitiş tarihi başlangıç tarihinden sonra olmalıdır.",
                [nameof(EndDate)]);
        }

        if (SelectedScreenIds.Count == 0)
        {
            yield return new ValidationResult(
                "En az bir ekran seçilmelidir.",
                [nameof(SelectedScreenIds)]);
        }
    }
}
