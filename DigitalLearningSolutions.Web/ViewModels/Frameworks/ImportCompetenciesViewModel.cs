namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public class ImportCompetenciesViewModel
    {
        public int FrameworkId { get; set; }
        [Required(ErrorMessage = "Import competencies file is required")]
        [AllowedExtensions(new[] { ".xlsx" }, "Import competencies file must be in xlsx format")]
        [MaxFileSize(5 * 1024 * 1024, "Maximum allowed file size is 5MB")]
        public IFormFile? ImportFile { get; set; }
    }
}
