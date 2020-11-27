namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class Logo
    {
        public readonly string? LogoUrl;
        public readonly string Name;

        public Logo(string logoName, byte[]? logoData, string logoMimeType)
        {
            if (logoData == null || logoData.Length < 10)
            {
                LogoUrl = null;
            }
            else
            {
                string base64Logo = Convert.ToBase64String(logoData);
                LogoUrl = $"data:{logoMimeType};base64,{base64Logo}";
            }

            Name = logoName;
        }

        public Logo(string logoName, byte[]? logoData) : this (logoName, logoData, "jpeg")
        {
        }
    }
}
