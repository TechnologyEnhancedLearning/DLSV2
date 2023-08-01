using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Models.Centres
{
    public class ContractInfo
    {
        public int CentreID { get; set; }
        public string CentreName { get; set; }
        public int ContractTypeID { get; set; }
        public string ContractType { get; set; }
        public long ServerSpaceBytesInc { get; set; }
        public long DelegateUploadSpace { get; set; }
        public DateTime? ContractReviewDate { get; set; }
    }
}
