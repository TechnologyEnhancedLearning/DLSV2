namespace DigitalLearningSolutions.Web.ViewModels.Common.MiniHub
{
    using System.Collections.Generic;

    public class MiniHubNavigationModel
    {
        public MiniHubNavigationModel(string miniHubName)
        {
            MiniHubName = miniHubName;
        }

        public readonly string MiniHubName;
        public List<MiniHubSection> Sections { get; set; } = new List<MiniHubSection>();
        public int CurrentSectionIndex { get; set; }
    }
}
