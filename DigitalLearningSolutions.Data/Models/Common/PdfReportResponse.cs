using DigitalLearningSolutions.Data.Models.External.Filtered;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Common
{
    public class PdfReportResponse
    {
        [JsonProperty("fileName")]
        public string? FileName { get; set; }
        [JsonProperty("hash")]
        public string? Hash { get; set; }
    }
}
