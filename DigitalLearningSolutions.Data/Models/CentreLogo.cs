namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class CentreLogo
    {
        public readonly string? LogoUrl;
        public readonly int Height;
        public readonly int Width;

        public CentreLogo(byte[]? logoData, int height, int width, string mimeType)
        {
            Height = height;
            Width = width;
            if (logoData == null)
            {
                LogoUrl = null;
            }
            else
            {
                string base64Logo = Convert.ToBase64String(logoData);
                LogoUrl = $"data:{mimeType};base64,{base64Logo}";
            }
        }
    }
}
