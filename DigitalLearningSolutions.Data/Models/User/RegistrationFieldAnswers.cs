﻿namespace DigitalLearningSolutions.Data.Models.User
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;

    public class RegistrationFieldAnswers
    {
        public RegistrationFieldAnswers(
            int centreId,
            int jobGroupId,
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
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            Answer5 = answer5;
            Answer6 = answer6;
        }

        public RegistrationFieldAnswers(DelegateDetailsData delegateDetailsData, int jobGroupId, int centreId)
        {
            Answer1 = delegateDetailsData.Answer1;
            Answer2 = delegateDetailsData.Answer2;
            Answer3 = delegateDetailsData.Answer3;
            Answer4 = delegateDetailsData.Answer4;
            Answer5 = delegateDetailsData.Answer5;
            Answer6 = delegateDetailsData.Answer6;
            JobGroupId = jobGroupId;
            CentreId = centreId;
        }

        public int CentreId { get; set; }
        public int JobGroupId { get; set; }
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
