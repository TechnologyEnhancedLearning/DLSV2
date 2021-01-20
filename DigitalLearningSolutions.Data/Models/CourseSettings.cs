namespace DigitalLearningSolutions.Data.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CourseSettings
    {
        public bool ShowPercentage { get; } = true;
        public bool ShowTime { get; } = true;
        public bool ShowLearnStatus { get; } = true;
        public string? ConsolidationExercise { get; } = null;
        public string? SupportingInformation { get; } = null;
        private const string BaseKey = "lm.";
        private readonly string ShowPercentageKey = $"{BaseKey}sp";
        private readonly string ShowTimeKey = $"{BaseKey}st";
        private readonly string ShowLearnStatusKey = $"{BaseKey}sl";
        private readonly string ShowConsolidationExerciseKey = $"{BaseKey}ce";
        private readonly string ShowSupportingInformationKey = $"{BaseKey}si";

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

                if (settings.ContainsKey(ShowPercentageKey) && settings[ShowPercentageKey] is bool showPercentage)
                {
                    ShowPercentage = showPercentage;
                }

                if (settings.ContainsKey(ShowTimeKey) && settings[ShowTimeKey] is bool showTime)
                {
                    ShowTime = showTime;
                }

                if (settings.ContainsKey(ShowLearnStatusKey) && settings[ShowLearnStatusKey] is bool showLearnStatus)
                {
                    ShowLearnStatus = showLearnStatus;
                }

                if (settings.ContainsKey(ShowConsolidationExerciseKey) && settings[ShowConsolidationExerciseKey] is string consolidationExercise)
                {
                    ConsolidationExercise = string.IsNullOrWhiteSpace(consolidationExercise)
                        ? null
                        : consolidationExercise;
                }

                if (settings.ContainsKey(ShowSupportingInformationKey) && settings[ShowSupportingInformationKey] is string supportingInformation)
                {
                    SupportingInformation = string.IsNullOrWhiteSpace(supportingInformation)
                        ? null
                        : supportingInformation;
                }
            }
            catch (JsonException)
            {
                // Catch JSON parsing errors and exit, as the fields are already set to their default values
            }
        }
    }
}
