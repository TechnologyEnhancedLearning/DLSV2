namespace DigitalLearningSolutions.Data.Models.User
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class DelegateUser : User
    {
        public string CandidateNumber { get; set; }
        public DateTime? DateRegistered { get; set; }
        public int JobGroupId { get; set; }
        public string? JobGroupName { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }
        public string? AliasId { get; set; }
        public bool HasBeenPromptedForPrn { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool HasDismissedLhLoginWarning { get; set; }

        public override string[] SearchableContent => new []{ SearchableName, CandidateNumber };

        public override UserReference ToUserReference()
        {
            return new UserReference(Id, UserType.DelegateUser);
        }

        public CentreAnswersData GetCentreAnswersData()
        {
            return new CentreAnswersData(
                CentreId,
                JobGroupId,
                Answer1,
                Answer2,
                Answer3,
                Answer4,
                Answer5,
                Answer6
            );
        }
    }
}
