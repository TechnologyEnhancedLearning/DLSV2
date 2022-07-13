using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class RemoveCustomFlagConfirmationViewModel
    {
        public int FlagId { get; set; }
        public int FrameworkId { get; set; }
        public string FlagName { get; set; }
    }
}
