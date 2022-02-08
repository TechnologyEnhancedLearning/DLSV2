namespace DigitalLearningSolutions.Data.Models.Supervisor
{
    using System;

    public class SupervisorDelegateDetail : SupervisorDelegate
    {
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
