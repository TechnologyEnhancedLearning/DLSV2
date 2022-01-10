namespace DigitalLearningSolutions.Web.Tests.ViewModels.SuperAdmin.Centres
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CentresViewModelTests
    {
        [Test]
        public void CentresViewModel_default_should_return_first_page_of_centres_in_ascending_order()
        {
            // Given
            var centres = new List<CentreSummaryForSuperAdmin>
            {
                new CentreSummaryForSuperAdmin { CentreName = "A" },
                new CentreSummaryForSuperAdmin { CentreName = "b" },
                new CentreSummaryForSuperAdmin { CentreName = "C" },
                new CentreSummaryForSuperAdmin { CentreName = "F" },
                new CentreSummaryForSuperAdmin { CentreName = "J" },
                new CentreSummaryForSuperAdmin { CentreName = "e" },
                new CentreSummaryForSuperAdmin { CentreName = "w" },
                new CentreSummaryForSuperAdmin { CentreName = "S" },
                new CentreSummaryForSuperAdmin { CentreName = "r" },
                new CentreSummaryForSuperAdmin { CentreName = "H" },
                new CentreSummaryForSuperAdmin { CentreName = "m" },
            };

            // When
            var model = new CentresViewModel(centres);

            // Then
            using (new AssertionScope())
            {
                model.Centres
                    .Should().HaveCount(10)
                    .And.BeInAscendingOrder(o => o.CentreName);
            }
        }
    }
}
