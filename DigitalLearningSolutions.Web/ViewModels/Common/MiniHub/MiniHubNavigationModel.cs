namespace DigitalLearningSolutions.Web.ViewModels.Common.MiniHub
{
    using System.Collections.Generic;

    public class MiniHubNavigationModel
    {
        public List<MiniHubSection> Sections { get; set; }
        public int CurrentSectionIndex { get; set; }
    }
}
