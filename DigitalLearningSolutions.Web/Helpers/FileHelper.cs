namespace DigitalLearningSolutions.Web.Helpers
{
    using Microsoft.AspNetCore.StaticFiles;

    public static class FileHelper
    {
        public static string? GetContentTypeFromFileName(string fileName)
        {
            return new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType)
                ? contentType
                : null;
        }
    }
}
