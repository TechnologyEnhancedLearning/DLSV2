namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;

    public class CentreAnswersData
    {
        public CentreAnswersData(
            int centreId,
            int jobGroupId,
            string? centreEmail,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        )
        {
            CentreId = centreId;
            JobGroupId = jobGroupId;
            CentreEmail = centreEmail;
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            Answer5 = answer5;
            Answer6 = answer6;
        }

        public int CentreId { get; set; }
        // TODO HEEDLS-887 Remove JobGroupId because it was moved to MyAccountDetailsData
        public int JobGroupId { get; set; }
        public string? CentreEmail { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }

        public string? GetAnswerForRegistrationPromptNumber(RegistrationField field)
        {
            return field.Id switch
            {
                1 => Answer1,
                2 => Answer2,
                3 => Answer3,
                4 => Answer4,
                5 => Answer5,
                6 => Answer6,
                _ => throw new InvalidPromptNumberException(),
            };
        }
    }
}
