namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CourseSettings
    {
        public bool ShowPercentage { get; } = true;
        public bool ShowTime { get; } = true;

        public CourseSettings(string? settingsText)
        {
            if (settingsText == null)
            {
                return;
            }

            try
            {
                var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(settingsText);

                if (settings.ContainsKey("lm.sp") && settings["lm.sp"] is bool)
                {
                    ShowPercentage = (bool)settings["lm.sp"];
                }

                if (settings.ContainsKey("lm.st") && settings["lm.st"] is bool)
                {
                    ShowTime = (bool)settings["lm.st"];
                }
            }
            catch (JsonException)
            {
                // Catch JSON parsing errors and exit, as the fields are already set to their default values
            }
        }
    }
}
