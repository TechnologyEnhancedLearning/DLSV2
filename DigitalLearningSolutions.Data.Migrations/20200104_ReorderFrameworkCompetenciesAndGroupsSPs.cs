using System;
using DevExpress.Xpo;

namespace DigitalLearningSolutions.Data.Migrations
{

    public class _20200104_ReorderFrameworkCompetenciesAndGroupsSPs : XPObject
    {
        public _20200104_ReorderFrameworkCompetenciesAndGroupsSPs() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public _20200104_ReorderFrameworkCompetenciesAndGroupsSPs(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }
    }

}