namespace DigitalLearningSolutions.Data.Models.External.Filtered
{
    using System.Collections.Generic;
    public class PlayList
    {
#pragma warning disable IDE1006 // Naming Styles
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<Competency> typeExtra { get; set; } = new List<Competency>();
        public List<LearningAsset> laList { get; set; } = new List<LearningAsset>();
#pragma warning restore IDE1006 // Naming Styles
    }
}
