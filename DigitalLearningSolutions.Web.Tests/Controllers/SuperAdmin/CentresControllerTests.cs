using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Web.Controllers.SuperAdmin.Centres;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
using DocumentFormat.OpenXml.EMMA;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace DigitalLearningSolutions.Web.Tests.Controllers.SuperAdmin
{
    public class CentresControllerTests
    {
        private readonly ICentresDataService centresDataService = A.Fake<ICentresDataService>();
        private readonly ICentresService centresService = A.Fake<ICentresService>();
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService = A.Fake<ISearchSortFilterPaginateService>();
        private readonly IRegionDataService regionDataService = A.Fake<IRegionDataService>();
        private readonly IContractTypesDataService contractTypesDataService = A.Fake<IContractTypesDataService>();
        private readonly ICourseDataService courseDataService = A.Fake<ICourseDataService>();
        private CentresController controller = null!;

        [SetUp]
        public void Setup()
        {
            controller = new CentresController(
            centresService,
            searchSortFilterPaginateService,
            regionDataService,
            centresDataService,
            contractTypesDataService,
            courseDataService
            )
            .WithDefaultContext()
            .WithMockUser(true);

            A.CallTo(() => centresDataService.UpdateCentreDetailsForSuperAdmin(
                    A<int>._,
                    A<string>._,
                    A<int>._,
                    A<int>._,
                    A<string>._,
                    A<string>._,
                    A<bool>._
                    )).DoesNothing();
        }

        [TearDown]
        public void Cleanup()
        {
            Fake.ClearRecordedCalls(centresDataService);
        }

        [Test]
        public void EditCentreDetailst_updates_centre_and_redirects_with_successful_save()
        {
            // Given
            var model = new EditCentreDetailsSuperAdminViewModel
            {
                CentreId = 374,
                CentreName = "##HEE Demo Centre##",
                CentreTypeId = 1,
                CentreType = "NHS Organisation",
                RegionName = "National",
                CentreEmail = "no.email@hee.nhs.uk",
                IpPrefix = "12.33.4",
                ShowOnMap = true,
                RegionId = 13
            };

            // When
            var result = controller.EditCentreDetails(model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("ManageCentre");
            A.CallTo(() => centresDataService.UpdateCentreDetailsForSuperAdmin(
                                                model.CentreId,
                                                model.CentreName,
                                                model.CentreTypeId,
                                                model.RegionId,
                                                model.CentreEmail,
                                                model.IpPrefix,
                                                model.ShowOnMap))
                                                .MustHaveHappenedOnceExactly();
        }
    }
}

