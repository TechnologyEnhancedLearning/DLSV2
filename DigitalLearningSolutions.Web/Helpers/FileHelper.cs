namespace DigitalLearningSolutions.Web.Helpers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.StaticFiles;
    using System.IO;
    using System;

    public static class FileHelper
    {
        public static string? GetContentTypeFromFileName(string fileName)
        {
            return new FileExtensionContentTypeProvider().TryGetContentType(fileName, out var contentType)
                ? contentType
                : null;
        }
        public static string UploadFile(IWebHostEnvironment webHostEnvironment, IFormFile file)
        {
            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
            string fileName = null;
            if (file != null)
            {
                fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        public static void DeleteFile(IWebHostEnvironment webHostEnvironment, string fileName)
        {
            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
            var filePath = Path.Combine(uploadDir, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static IFormFile LoadFileFromPath(IWebHostEnvironment webHostEnvironment, string fileName)
        {
            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
            var filePath = Path.Combine(uploadDir, fileName);
            // Read the file into a byte array
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Create a memory stream from the byte array
            using (var memoryStream = new MemoryStream(fileBytes))
            {
                // Create an IFormFile instance using the memory stream
                return new FormFile(memoryStream, 0, fileBytes.Length, Path.GetFileName(filePath), Path.GetFileName(filePath));
            }
        }
    }
}
