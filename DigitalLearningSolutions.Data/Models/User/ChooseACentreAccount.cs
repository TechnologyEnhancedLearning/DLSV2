﻿namespace DigitalLearningSolutions.Data.Models.User
{
    public class ChooseACentreAccount
    {
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDelegate { get; set; }
    }
}
