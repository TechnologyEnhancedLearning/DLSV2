namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
using System;
    public class ContractTypeViewModel
    {
        public ContractTypeViewModel()
        {

        }
        public ContractTypeViewModel(
            int centreId, string centreName, int contractTypeID,
            string contractType, long serverSpaceBytesInc,
            long DelegateUploadSpace, DateTime? ContractReviewDate
            )
        {
            this.CentreId = centreId;
            this.CentreName = centreName;
            this.ContractTypeID = contractTypeID;
            this.ContractReviewDate = ContractReviewDate;
            this.DelegateUploadSpace = DelegateUploadSpace;
            this.ContractType = contractType;
            this.ServerSpaceBytesInc = serverSpaceBytesInc;
        }
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public int ContractTypeID { get; set; }
        public string ContractType { get; set; }
        public long ServerSpaceBytesInc { get; set; }
        public long DelegateUploadSpace { get; set; }
        public DateTime? ContractReviewDate { get; set; }
    }
}
