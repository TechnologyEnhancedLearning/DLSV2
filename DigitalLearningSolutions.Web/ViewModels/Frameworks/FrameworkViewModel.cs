namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class FrameworkViewModel
    {
        public TabsNavViewModel TabNavLinks { get; set; }
        public DetailFramework? DetailFramework { get; set; }
        public IEnumerable<CollaboratorDetail>? Collaborators { get; set; }
        public List<FrameworkCompetencyGroup>? FrameworkCompetencyGroups { get; set; }
        public IEnumerable<FrameworkCompetency>? FrameworkCompetencies { get; set; }
        public IEnumerable<AssessmentQuestion>? FrameworkDefaultQuestions { get; set; }
        public IEnumerable<CommentReplies>? CommentReplies { get; set; }
        [BindProperty]
        [StringLength(255000, MinimumLength = 3)]
        [Required]
        public string Comment { get; set; } = "";
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
