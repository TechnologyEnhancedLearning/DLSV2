namespace DigitalLearningSolutions.Data.Models.Register
{
    public class DelegateRegistrationModel
    {
        public DelegateRegistrationModel(
            string firstName,
            string lastName,
            string email,
            int centre,
            int jobGroup,
            string passwordHash,
            string? answer1,
            string? answer2,
            string? answer3,
            string? answer4,
            string? answer5,
            string? answer6)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Centre = centre;
            JobGroup = jobGroup;
            PasswordHash = passwordHash;
            Approved = false;
            Answer1 = answer1;
            Answer2 = answer2;
            Answer3 = answer3;
            Answer4 = answer4;
            Answer5 = answer5;
            Answer6 = answer6;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public int Centre { get; set; }

        public int JobGroup { get; set; }

        public string PasswordHash { get; set; }

        public bool Approved { get; set; }

        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }
    }
}
