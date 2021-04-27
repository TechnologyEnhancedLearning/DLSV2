namespace DigitalLearningSolutions.Web.ViewModels.Common.MiniHub
{
    using System.Collections.Generic;
    using System.Linq;

    public class MiniHubNavigationModel
    {
        public MiniHubNavigationModel(string miniHubName, IEnumerable<MiniHubSection> sections, int currentSectionIndex)
        {
            MiniHubName = miniHubName;
            Sections = sections.ToList();
            CurrentSectionIndex = currentSectionIndex;
        }

        public readonly string MiniHubName;
        public readonly List<MiniHubSection> Sections;
        public readonly int CurrentSectionIndex;
    }
}
