using DigitalLearningSolutions.Data.Models.Supervisor;
using DigitalLearningSolutions.Data.Utilities;
using System;

namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    public static class SupervisorTagTestHelper
    {
        private static readonly IClockUtility ClockUtility = new ClockUtility();

        public static SupervisorDelegateDetail CreateDefaultSupervisorDelegateDetail(
             int id =1,
          string supervisorEmail = "email@test.com",
          string SupervisorName  = "Supervisor",
          int? supervisorAdminID = 1,
          int centreId = 101,
          string delegateEmail = "email@test.com",
          int? delegateUserID = 1,
          bool addedByDelegate = false,
          DateTime? removed = null,
          string? firstName = null,
          string? lastName = null,
        string candidateNumber  = "DELEGATE",
        string candidateEmail = "email@test.com",
         string? jobGroupName =null,
         string? customPrompt1 =null,
         string? answer1 = null,
         string? customPrompt2 = null,
          string? answer2 = null,
          string? customPrompt3 = null,
          string? answer3 = null,
          string? customPrompt4 = null,
          string? answer4 = null,
          string? customPrompt5 = null,
          string? answer5 = null,
          string? customPrompt6 = null,
          string? answer6 = null,
           string? supervisorName = null,
          int candidateAssessmentCount =0,
          Guid? InviteHash =null,
          bool delegateIsNominatedSupervisor = false,
          bool delegateIsSupervisor = false,
          string professionalRegistrationNumber = "string.Empty",
          int? delegateID = 0,
          bool? active =false
       )
        {
            return new SupervisorDelegateDetail
            {
                ID = id,
                Active = active,
                FirstName = firstName,
                LastName = lastName,
                CentreId = centreId,
               CandidateAssessmentCount = candidateAssessmentCount,
               CandidateNumber = candidateNumber,
               CandidateEmail = candidateEmail,
               Answer1 = answer1,
               Answer2 = answer2,
               Answer3 = answer3,
               Answer4 = answer4,
               Answer5 = answer5,
               Answer6 = answer6,
               JobGroupName = jobGroupName,
               DelegateEmail = delegateEmail,
               DelegateID = delegateID,
               DelegateIsNominatedSupervisor= delegateIsNominatedSupervisor,
               DelegateIsSupervisor= delegateIsSupervisor,
               DelegateUserID = delegateUserID,
                SupervisorAdminID = supervisorAdminID,
                SupervisorEmail= supervisorEmail,
                SupervisorName = supervisorName,
                CustomPrompt1 = customPrompt1,
                CustomPrompt2 = customPrompt2,
                CustomPrompt3 = customPrompt3,
                CustomPrompt4 = customPrompt4,
                CustomPrompt5 = customPrompt5,
                CustomPrompt6 = customPrompt6,
                Removed = removed,
                InviteHash = InviteHash,
                ProfessionalRegistrationNumber = professionalRegistrationNumber,
                
            };
        }

        public static DelegateSelfAssessment CreateDefaultDelegateSelfAssessment(
                            int id = 1,
          int selfAssessmentID =6,
          int delegateUserID =1,
          string? roleName =null,
          bool supervisorSelfAssessmentReview =false,
          bool supervisorResultsReview = false,
          string? supervisorRoleTitle = "Assessor",
          DateTime? signedOffDate =null,
          bool signedOff = false,
          DateTime? completeByDate=null,
          int launchCount = 0,
          DateTime? completedDate =null,
          string? professionalGroup = null,
          string? questionLabel = null,
          string? descriptionLabel = null,
          string? reviewerCommentsLabel = null,
          string? subGroup = null,
          string? roleProfile = null,
          int signOffRequested =1,
          int resultsVerificationRequests =1,
          bool isSupervisorResultsReviewed =false,
          bool isAssignedToSupervisor = false,
          bool nonReportable = false
            )
        {
            return new DelegateSelfAssessment {
                ID = id,
                SelfAssessmentID = selfAssessmentID,
                DelegateUserID = delegateUserID,
                ResultsVerificationRequests = resultsVerificationRequests,
                ReviewerCommentsLabel  = reviewerCommentsLabel,
                SubGroup = subGroup,
                RoleProfile = roleProfile,
                SignOffRequested = signOffRequested,
                SupervisorResultsReview = supervisorResultsReview,
                SupervisorSelfAssessmentReview= supervisorSelfAssessmentReview,
                SignedOff = signedOff,
                CompleteByDate = completeByDate,
                LaunchCount = launchCount,
                CompletedDate = completedDate,
                SignedOffDate = signedOffDate,
                ProfessionalGroup = professionalGroup,
                QuestionLabel = questionLabel,
                DescriptionLabel = descriptionLabel,
                IsAssignedToSupervisor = isAssignedToSupervisor,
                NonReportable = nonReportable,
                IsSupervisorResultsReviewed= isSupervisorResultsReviewed,
                RoleName = roleName,
                SupervisorRoleTitle = supervisorRoleTitle,
                
            };
        }

    }
}
