namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;

    public class PublishViewModel
    {
        public DetailFramework DetailFramework { get; set; }
        public IEnumerable<FrameworkReview>? FrameworkReviews { get; set; }
        public bool CanPublish { get; set; }
        public string VocabSingular()
        {
            return FrameworkVocabularyHelper.VocabularySingular(DetailFramework.FrameworkConfig);
        }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(DetailFramework.FrameworkConfig);
        }
    }
}
