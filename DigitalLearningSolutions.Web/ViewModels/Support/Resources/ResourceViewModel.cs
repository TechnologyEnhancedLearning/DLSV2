namespace DigitalLearningSolutions.Web.ViewModels.Support.Resources
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Helpers;

    public class ResourceViewModel
    {
        private readonly string currentSystemBaseUrl;
        private readonly string downloadTag;

        public ResourceViewModel(Resource resource, string currentSystemBaseUrl)
        {
            this.currentSystemBaseUrl = currentSystemBaseUrl;
            var fileNameParts = resource.FileName.Split('.');
            Resource = $"{resource.Description} (.{fileNameParts.Last()})";
            Date = resource.UploadDateTime.ToString(DateHelper.StandardDateFormat);
            Size = DisplayStringHelper.GenerateSizeDisplayString(resource.FileSize);
            downloadTag = resource.Tag;
        }

        public string Resource { get; }
        public string Date { get; }
        public string Size { get; }
        public string DownloadUrl => $"{currentSystemBaseUrl}/tracking/download?content={downloadTag}";
    }
}
