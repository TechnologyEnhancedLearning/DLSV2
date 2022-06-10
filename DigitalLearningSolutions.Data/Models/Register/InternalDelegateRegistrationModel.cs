namespace DigitalLearningSolutions.Data.Models.Register
{
    public class InternalDelegateRegistrationModel
    {
        public InternalDelegateRegistrationModel(
            int centre,
            string? centreSpecificEmail,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6
        )
        {
            Centre = centre;
            CentreSpecificEmail = centreSpecificEmail;
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            Answer5 = answer5;
            Answer6 = answer6;
        }

        public string? CentreSpecificEmail { get; set; }

        public int Centre { get; set; }

        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }
    }
}
