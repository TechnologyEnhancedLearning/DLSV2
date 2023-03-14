using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Common
{
    public class PdfReportStatusResponse
    {
        [JsonProperty("id")]
        public int? Id { get; set; }
        [JsonProperty("status")]
        public string? Status { get; set; }
    }
}
