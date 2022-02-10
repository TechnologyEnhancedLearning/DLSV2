namespace DigitalLearningSolutions.Web.Tests.ViewModels.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CentreSummaryViewModelTests
    {
        [Test]
        public void CentreSummaryViewModel_constructor_should_populate_expected_properties()
        {
            // Given
            var centre = new CentreSummaryForSuperAdmin()
            {
                CentreId = 2,
                CentreName = "North West Boroughs Healthcare NHS Foundation Trust",
                RegionId = 5,
                RegionName = "North West",
                ContactForename = "TestForename",
                ContactSurname = "TestSurname",
                ContactEmail = "email@nhs.net",
                ContactTelephone = "0123654789",
                CentreTypeId = 4,
                CentreType = "Social Care",
                Active = true,
            };

            // When
            var model = new CentreSummaryViewModel(centre);

            // Then
            using (new AssertionScope())
            {
                model.CentreId.Should().Be(2);
                model.CentreName.Should().Be("North West Boroughs Healthcare NHS Foundation Trust");
                model.RegionName.Should().Be("North West");
                model.ContactForename.Should().Be("TestForename");
                model.ContactSurname.Should().Be("TestSurname");
                model.ContactEmail.Should().Be("email@nhs.net");
                model.ContactTelephone.Should().Be("0123654789");
                model.CentreType.Should().Be("Social Care");
                model.Active.Should().BeTrue();
            }
        }
    }
}
