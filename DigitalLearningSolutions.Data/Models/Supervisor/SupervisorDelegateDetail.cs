// QQ fix line endings before merge

namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    using System;

    public class SupervisorDelegateDetail : SupervisorDelegate
    {
        public SupervisorDelegateDetail() { }

        public SupervisorDelegateDetail(SupervisorDelegateDetail supervisorDelegateDetail) : base(
            supervisorDelegateDetail
        )
        {
            CandidateNumber = supervisorDelegateDetail.CandidateNumber;
            JobGroupName = supervisorDelegateDetail.JobGroupName;
            CustomPrompt1 = supervisorDelegateDetail.CustomPrompt1;
            Answer1 = supervisorDelegateDetail.Answer1;
            CustomPrompt2 = supervisorDelegateDetail.CustomPrompt2;
            Answer2 = supervisorDelegateDetail.Answer2;
            CustomPrompt3 = supervisorDelegateDetail.CustomPrompt3;
            Answer3 = supervisorDelegateDetail.Answer3;
            CustomPrompt4 = supervisorDelegateDetail.CustomPrompt4;
            Answer4 = supervisorDelegateDetail.Answer4;
            CustomPrompt5 = supervisorDelegateDetail.CustomPrompt5;
            Answer5 = supervisorDelegateDetail.Answer5;
            CustomPrompt6 = supervisorDelegateDetail.CustomPrompt6;
            Answer6 = supervisorDelegateDetail.Answer6;
            ProfileImage = supervisorDelegateDetail.ProfileImage;
            SupervisorName = supervisorDelegateDetail.SupervisorName;
            CandidateAssessmentCount = supervisorDelegateDetail.CandidateAssessmentCount;
            InviteHash = supervisorDelegateDetail.InviteHash;
        }

        public string CandidateNumber { get; set; }
        public string? JobGroupName { get; set; }
        public string? CustomPrompt1 { get; set; }
        public string? Answer1 { get; set; }
        public string? CustomPrompt2 { get; set; }
        public string? Answer2 { get; set; }
        public string? CustomPrompt3 { get; set; }
        public string? Answer3 { get; set; }
        public string? CustomPrompt4 { get; set; }
        public string? Answer4 { get; set; }
        public string? CustomPrompt5 { get; set; }
        public string? Answer5 { get; set; }
        public string? CustomPrompt6 { get; set; }
        public string? Answer6 { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? SupervisorName { get; set; }
        public int CandidateAssessmentCount { get; set; }
        public Guid? InviteHash { get; set; }
    }
}
