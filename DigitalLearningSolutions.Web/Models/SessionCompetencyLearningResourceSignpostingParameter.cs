using System;
using System.Collections.Generic;
using DigitalLearningSolutions.Data.Models.Frameworks;
using Microsoft.AspNetCore.Http;
using DigitalLearningSolutions.Web.Models.Enums;

namespace DigitalLearningSolutions.Web.Models
{
    public class SessionCompetencyLearningResourceSignpostingParameter
    {
        public Guid Id { get; set; }
        public LearningResourceReference LearningResourceReference { get; set; }
        public List<AssessmentQuestion> Questions { get; set; }
        public AssessmentQuestion SelectedQuestion { get; set; }
        public int[] SelectedLevelValues { get; set; }
        public CompetencyResourceAssessmentQuestionParameter AssessmentQuestionParameter { get; set; }
        public FrameworkCompetency Competency { get; set; }

        public CompareAssessmentQuestionType? SelectedCompareQuestionType { get; set; }
        public AssessmentQuestion SelectedCompareToQuestion { get; set; }
        public List<LevelDescriptor> LevelDescriptors { get; set; }

        public SessionCompetencyLearningResourceSignpostingParameter()
        {
        }
        public SessionCompetencyLearningResourceSignpostingParameter(string cookieName, IRequestCookieCollection requestCookies, IResponseCookies responseCookies, FrameworkCompetency competency, LearningResourceReference resource, List<AssessmentQuestion> questions, CompetencyResourceAssessmentQuestionParameter assessmentQuestionParameter)
        {
            var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) };
            Competency = competency;
            LearningResourceReference = resource;
            Questions = questions;
            AssessmentQuestionParameter = assessmentQuestionParameter;

            if (requestCookies.ContainsKey(cookieName) && requestCookies.TryGetValue(cookieName, out string id))
            {
                this.Id = Guid.Parse(id);
            }
            else
            {
                var guid = Guid.NewGuid();
                responseCookies.Append(cookieName, guid.ToString(), options);
                this.Id = guid;
            }
        }
    }
}
