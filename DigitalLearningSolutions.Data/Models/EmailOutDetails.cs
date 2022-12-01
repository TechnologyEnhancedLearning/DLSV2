namespace DigitalLearningSolutions.Data.Models
{
    using System;
    using DigitalLearningSolutions.Data.Utilities;

    public class EmailOutDetails
    {
        public int EmailID { get; set; }
        public string EmailTo { get; set; }
        public DateTime Added { get; set; }
        public DateTime Sent { get; set; }
        public DateTime DeliverAfter { get; set; }
    }
}
