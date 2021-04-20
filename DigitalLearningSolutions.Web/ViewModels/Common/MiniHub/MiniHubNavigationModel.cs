namespace DigitalLearningSolutions.Web.ViewModels.Common.MiniHub
{
    using System.Collections.Generic;

    public class MiniHubNavigationModel
    {
        public List<MiniHubSection> Sections { get; set; } = new List<MiniHubSection>();
        public int CurrentSectionIndex { get; set; }
    }
}
