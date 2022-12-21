namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    using DigitalLearningSolutions.Data.Enums;
    using System;

    public class SupervisorDelegateDetail : SupervisorDelegate
    {
        public string CandidateNumber { get; set; }
        public string CandidateEmail { get; set; }
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
        public bool DelegateIsNominatedSupervisor { get; set; }
        public bool DelegateIsSupervisor { get; set; }
        public string ProfessionalRegistrationNumber { get; set; }
        public string? DelegateUserId { get; set; }
        public DlsRole DlsRole
        {
            get
            {
                if (DelegateIsSupervisor)
                    return DlsRole.Supervisor;
                else if (DelegateIsNominatedSupervisor)
                    return DlsRole.NominatedSupervisor;
                else
                    return DlsRole.Learner;
            }
        }
    }
}
