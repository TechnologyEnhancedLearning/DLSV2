
namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System;
    using System.Collections.Generic;

    public class ContractTypeViewModel
    {
        public ContractTypeViewModel()
        {

        }
        public ContractTypeViewModel(
            int centreId, string centreName, int contractTypeID,
            string contractType, long serverSpaceBytesInc,
            long DelegateUploadSpace, DateTime? ContractReviewDate,
            int? contractReviewDay, int? contractReviewMonth, int? contractReviewYear
            )
        {
            this.CentreId = centreId;
            this.CentreName = centreName;
            this.ContractTypeID = contractTypeID;
            this.ContractReviewDate = ContractReviewDate;
            this.DelegateUploadSpace = DelegateUploadSpace;
            this.ContractType = contractType;
            this.ServerSpaceBytesInc = serverSpaceBytesInc;
            this.ContractReviewDay = contractReviewDay;
            this.ContractReviewMonth = contractReviewMonth;
            this.ContractReviewYear = contractReviewYear;
        }
        public int CentreId { get; set; }
        public string CentreName { get; set; }
        public int ContractTypeID { get; set; }
        public string ContractType { get; set; }
        public long ServerSpaceBytesInc { get; set; }
        public long DelegateUploadSpace { get; set; }
        public DateTime? ContractReviewDate { get; set; }
        public int? ContractReviewDay { get; set; }
        public int? ContractReviewMonth { get; set; }
        public int? ContractReviewYear { get; set; }
        public OldDateValidator.ValidationResult? CompleteByValidationResult { get; set; }
        public IEnumerable<SelectListItem> ContractTypeOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ServerSpaceOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> PerDelegateUploadSpaceOptions { get; set; } = new List<SelectListItem>();

    }
}
