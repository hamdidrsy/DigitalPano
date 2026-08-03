using System.ComponentModel.DataAnnotations;

namespace DigitalPano.Web.Models.Admin.Tickers;

public sealed class TickerMessageFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kayan yazı metni zorunludur.")]
    [StringLength(1000, ErrorMessage = "Kayan yazı en fazla 1000 karakter olabilir.")]
    [Display(Name = "Mesaj")]
    public string Text { get; set; } = string.Empty;

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
    [Display(Name = "Yayın başlangıcı")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
    [Display(Name = "Yayın bitişi")]
    public DateTime EndDate { get; set; }

    [Range(-10000, 10000, ErrorMessage = "Sıra -10000 ile 10000 arasında olmalıdır.")]
    [Display(Name = "Sıra")]
    public int SortOrder { get; set; }

    [Display(Name = "Yayına açık")]
    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
        {
            yield return new ValidationResult(
                "Bitiş tarihi başlangıç tarihinden sonra olmalıdır.",
                [nameof(EndDate)]);
        }
    }
}
