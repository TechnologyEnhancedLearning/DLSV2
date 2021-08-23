namespace DigitalLearningSolutions.Data.Models.User
{
    using System.Collections.Generic;

    public class CentreUserDetails
    {
        public CentreUserDetails(int centreId, string centreName, bool isAdmin = false, bool isDelegate = false)
        {
            CentreId = centreId;
            CentreName = centreName;
            IsAdmin = isAdmin;
            IsDelegate = isDelegate;
        }

        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDelegate { get; set; }

        public string TagString
        {
            get
            {
                var tags = new List<string>();

                if (IsAdmin)
                {
                    tags.Add("Admin");
                }

                if (IsDelegate)
                {
                    tags.Add("Delegate");
                }

                return string.Join(", ", tags);
            }
        }
    }
}
