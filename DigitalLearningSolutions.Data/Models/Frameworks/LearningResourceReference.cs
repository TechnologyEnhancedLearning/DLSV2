using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class LearningResourceReference
    {
        public int Id { get; set; }
        public int ResourceRefID { get; set; }
        public string OriginalResourceName { get; set; } = string.Empty;
        public int AdminID { get; set; }
        public DateTime Added { get; set; }
    }
}
