namespace DigitalLearningSolutions.Web.ViewModels.Support.Resources
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Helpers;

    public class ResourcesItemViewModel
    {
        private readonly string currentSystemBaseUrl;
        private readonly string downloadTag;

        public ResourcesItemViewModel(Resource resource, string currentSystemBaseUrl)
        {
            this.currentSystemBaseUrl = currentSystemBaseUrl;
            var fileNameParts = resource.FileName.Split('.');
            Resource = $"{resource.Description} (.{fileNameParts.Last()})";
            Date = resource.UploadDate.ToString(DateHelper.StandardDateFormat);
            Size = DisplayStringHelper.GenerateBytesDisplayString(resource.FileSize);
            downloadTag = resource.Tag;
        }

        public string Resource { get; }
        public string Date { get; }
        public string Size { get; }
        public string DownloadUrl => $"{currentSystemBaseUrl}/tracking/download?content={downloadTag}";
    }
}
