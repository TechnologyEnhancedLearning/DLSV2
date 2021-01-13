namespace DigitalLearningSolutions.Data.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CourseSettings
    {
        public bool ShowPercentage { get; } = true;
        public bool ShowTime { get; } = true;
        public bool ShowLearnStatus { get; } = true;
        public string? ConsolidationExercise { get; } = null;
        public string? SupportingInformation { get; } = null;

        public CourseSettings(string? settingsText)
        {
            if (settingsText == null)
            {
                return;
            }

            try
            {
                var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(settingsText);

                if (settings == null)
                {
                    return;
                }

                if (settings.ContainsKey("lm.sp") && settings["lm.sp"] is bool)
                {
                    ShowPercentage = (bool)settings["lm.sp"];
                }

                if (settings.ContainsKey("lm.st") && settings["lm.st"] is bool)
                {
                    ShowTime = (bool)settings["lm.st"];
                }

                if (settings.ContainsKey("lm.sl") && settings["lm.sl"] is bool)
                {
                    ShowLearnStatus = (bool)settings["lm.sl"];
                }

                if (settings.ContainsKey("lm:ce") && settings["lm:ce"] is string)
                {
                    ConsolidationExercise = (string)settings["lm:ce"];
                }

                if (settings.ContainsKey("lm:si") && settings["lm:si"] is string)
                {
                    SupportingInformation = (string)settings["lm:si"];
                }
            }
            catch (JsonException)
            {
                // Catch JSON parsing errors and exit, as the fields are already set to their default values
            }
        }
    }
}
