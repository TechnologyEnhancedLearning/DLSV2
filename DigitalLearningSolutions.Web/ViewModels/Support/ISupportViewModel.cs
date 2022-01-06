namespace DigitalLearningSolutions.Web.ViewModels.Support
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public interface ISupportViewModel
    {
        SupportPage CurrentPage { get; set; }
        DlsSubApplication DlsSubApplication { get; set; }
        string CurrentSystemBaseUrl { get; set; }
    }
}
