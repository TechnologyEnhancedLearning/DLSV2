namespace DigitalLearningSolutions.Web.ViewModels.Supervisor
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Supervisor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class MyStaffListViewModel
    {
        public IEnumerable<SupervisorDelegateDetail> SuperviseDelegateDetails { get; set; }
        public CentreCustomPrompts CentreCustomPrompts { get; set; }
    }
}
