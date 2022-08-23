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
        public string ResourceName { get; set; }
        public List<AssessmentQuestion> Questions { get; set; }
        public AssessmentQuestion SelectedQuestion { get; set; }
        public int SelectedQuestionRoleRequirements { get; set; }
        public int[] SelectedLevelValues { get; set; }
        public CompetencyResourceAssessmentQuestionParameter AssessmentQuestionParameter { get; set; }
        public FrameworkCompetency FrameworkCompetency { get; set; }

        public CompareAssessmentQuestionType? SelectedCompareQuestionType { get; set; }
        public List<LevelDescriptor> LevelDescriptors { get; set; }
        public bool TriggerValuesConfirmed { get; set; }
        public bool CompareQuestionConfirmed { get; set; }

        public SessionCompetencyLearningResourceSignpostingParameter()
        {
        }
        public SessionCompetencyLearningResourceSignpostingParameter(FrameworkCompetency frameworkCompetency, string resourceName, List<AssessmentQuestion> questions, AssessmentQuestion selectedQuestion, CompareAssessmentQuestionType selectedCompareQuestionType, CompetencyResourceAssessmentQuestionParameter assessmentQuestionParameter)
        {
            FrameworkCompetency = frameworkCompetency;
            ResourceName = resourceName;
            Questions = questions;
            AssessmentQuestionParameter = assessmentQuestionParameter;
            SelectedQuestion = selectedQuestion;
            SelectedCompareQuestionType = selectedCompareQuestionType;
            TriggerValuesConfirmed = false;
            CompareQuestionConfirmed = false;
        }
    }
}
