namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class FrameworkViewModel
    {
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
        public string vocabSingular()
        {
            if (DetailFramework.FrameworkConfig == null)
            {
                return "Capability";
            }
            else
            {
                return DetailFramework.FrameworkConfig;
            }
        }
        public string vocabPlural()
        {
            if (DetailFramework.FrameworkConfig == null)
            {
                return "Capabilities";
            }
            else
            {
                if (DetailFramework.FrameworkConfig.EndsWith("y"))
                {
                    return DetailFramework.FrameworkConfig.Substring(0, DetailFramework.FrameworkConfig.Length - 1) + "ies";
                }
                else
                {
                    return DetailFramework.FrameworkConfig + "s";
                }
            }
        }
    }
}
