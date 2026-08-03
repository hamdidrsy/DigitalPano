using System.ComponentModel.DataAnnotations;

namespace DigitalPano.Web.Models.Admin.Media;

public sealed class MediaUploadViewModel
{
    [Required(ErrorMessage = "Yüklenecek dosyayı seçiniz.")]
    [Display(Name = "Medya dosyası")]
    public IFormFile? File { get; set; }
}
