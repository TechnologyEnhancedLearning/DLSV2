namespace DigitalLearningSolutions.Web.Tests.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;
    using System.Collections.Generic;

    public class CentresViewModelTests
    {
        [Test]
        public void CentresViewModel_default_should_return_first_page_of_centres_in_ascending_order()
        {
            // Given
            var centres = new List<Centre>
            {
                new Centre { CentreName = "A" },
                new Centre { CentreName = "b" },
                new Centre { CentreName = "C" },
                new Centre { CentreName = "F" },
                new Centre { CentreName = "J" },
                new Centre { CentreName = "e" },
                new Centre { CentreName = "w" },
                new Centre { CentreName = "S" },
                new Centre { CentreName = "r" },
                new Centre { CentreName = "H" },
                new Centre { CentreName = "m" },
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
