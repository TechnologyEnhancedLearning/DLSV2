using System;

namespace DigitalLearningSolutions.Data.Models.Centres
{
    public class ContractInfo
    {
        public int CentreID { get; set; }
        public string CentreName { get; set; } = string.Empty;
        public int ContractTypeID { get; set; }
        public string ContractType { get; set; } = string.Empty;
        public long ServerSpaceBytesInc { get; set; }
        public long DelegateUploadSpace { get; set; }
        public DateTime? ContractReviewDate { get; set; }
    }
}
