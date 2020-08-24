namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class CentreLogo
    {
        public readonly string? LogoUrl;
        public int Height { get; private set; }
        public int Width { get; private set; }
        public readonly string CentreName;

        private const double MaxWidth = 280;
        private const double MaxHeight = 75;

        public CentreLogo(byte[]? logoData, int height, int width, string mimeType, string centreName)
        {
            Height = height;
            Width = width;
            ScaleImage();
            if (logoData == null || logoData.Length < 10)
            {
                LogoUrl = null;
            }
            else
            {
                string base64Logo = Convert.ToBase64String(logoData);
                LogoUrl = $"data:{mimeType};base64,{base64Logo}";
            }

            CentreName = centreName;
        }

        private void ScaleImage()
        {
            if (Width > MaxWidth)
            {
                Height = Convert.ToInt32(Height * (MaxWidth / Width));
                Width = Convert.ToInt32(MaxWidth);
            }

            if (Height > MaxHeight)
            {
                Width = Convert.ToInt32(Width * (MaxHeight / Height));
                Height = Convert.ToInt32(MaxHeight);
            }
        }
    }
}
