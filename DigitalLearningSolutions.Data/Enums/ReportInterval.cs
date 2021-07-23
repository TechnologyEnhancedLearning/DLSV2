using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Enums
{
    class ReportInterval : Enumeration
    {
        public static readonly ReportInterval Days = new ReportInterval(0, nameof(Days));
        public static readonly ReportInterval Weeks = new ReportInterval(1, nameof(Weeks));
        public static readonly ReportInterval Months = new ReportInterval(1, nameof(Months));
        public static readonly ReportInterval Quarters = new ReportInterval(1, nameof(Quarters));
        public static readonly ReportInterval Years = new ReportInterval(1, nameof(Years));

        public ReportInterval(int id, string name) : base(id, name)
        { }
    }
}
