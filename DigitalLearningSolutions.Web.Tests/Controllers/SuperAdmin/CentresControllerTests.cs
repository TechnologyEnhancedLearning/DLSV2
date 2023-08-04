using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Centres;
using DigitalLearningSolutions.Web.Controllers.SuperAdmin.Centres;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
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
        public void EditCentreDetails_updates_centre_and_redirects_with_successful_save()
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

        [Test]
        public void CentreRoleLimits_route_loads_existing_role_limits_with_derived_flags_set()
        {
            // Given
            var roleLimits = new CentreSummaryForRoleLimits
            {
                CentreId = 374,
                RoleLimitCmsAdministrators = -1,    // not set
                RoleLimitCmsManagers = -1,          // not set
                RoleLimitCcLicenses = 10,           // set
                RoleLimitCustomCourses = 20,        // set
                RoleLimitTrainers = 30,             // set
            };

            A.CallTo(() => centresDataService.GetRoleLimitsForCentre(
                A<int>._
            )).Returns(roleLimits);

            var expectedVm = new CentreRoleLimitsViewModel
            {
                CentreId = 374,
                RoleLimitCmsAdministrators = -1,
                IsRoleLimitSetCmsAdministrators = false,    // automatically set off
                RoleLimitCmsManagers = -1,
                IsRoleLimitSetCmsManagers = false,          // automatically set off
                IsRoleLimitSetContentCreatorLicenses = true,
                RoleLimitContentCreatorLicenses = 10,
                IsRoleLimitSetCustomCourses = true,
                RoleLimitCustomCourses = 20,
                IsRoleLimitSetTrainers = true,
                RoleLimitTrainers = 30,
            };

            // When
            var result = controller.CentreRoleLimits(374);

            // Then

            A.CallTo(() => centresDataService.GetRoleLimitsForCentre(374)).MustHaveHappenedOnceExactly();
            result.Should().BeViewResult("CentreRoleLimits").ModelAs<CentreRoleLimitsViewModel>().Should().BeEquivalentTo(expectedVm);
        }

        [Test]
        public void EditCentreRoleLimits_updates_centre_role_limits_and_redirects_with_successful_save()
        {
            // Given
            var model = new CentreRoleLimitsViewModel
            {
                CentreId = 374,
                IsRoleLimitSetCmsAdministrators = true,
                IsRoleLimitSetCmsManagers = false,
                IsRoleLimitSetContentCreatorLicenses = true,
                IsRoleLimitSetCustomCourses = false,
                IsRoleLimitSetTrainers = true,
                RoleLimitCmsAdministrators = 1,
                RoleLimitCmsManagers = -1,
                RoleLimitContentCreatorLicenses = 2,
                RoleLimitCustomCourses = -1,
                RoleLimitTrainers = 0
            };

            // When
            var result = controller.EditCentreRoleLimits(model);

            // Then
            A.CallTo(
                    () => centresDataService.UpdateCentreRoleLimits(
                        model.CentreId,
                        model.RoleLimitCmsAdministrators,
                        model.RoleLimitCmsManagers,
                        model.RoleLimitContentCreatorLicenses,
                        model.RoleLimitCustomCourses,
                        model.RoleLimitTrainers
                    )
                )
                .MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("ManageCentre").WithRouteValue("centreId", model.CentreId);
        }

        [Test]
        public void EditCentreRoleLimits_default_role_limit_value_to_negative_if_not_set()
        {
            // Given
            var model = new CentreRoleLimitsViewModel
            {
                CentreId = 374,
                IsRoleLimitSetCmsAdministrators = true,
                IsRoleLimitSetCmsManagers = false,
                IsRoleLimitSetContentCreatorLicenses = false,
                IsRoleLimitSetCustomCourses = false,
                IsRoleLimitSetTrainers = true,
                RoleLimitCmsAdministrators = 1,
                RoleLimitCmsManagers = 10,
                RoleLimitContentCreatorLicenses = 20,
                RoleLimitCustomCourses = 30,
                RoleLimitTrainers = -1,
            };

            // When
            var result = controller.EditCentreRoleLimits(model);

            // Then
            A.CallTo(
                    () => centresDataService.UpdateCentreRoleLimits(
                        model.CentreId,
                        model.RoleLimitCmsAdministrators,
                        -1,
                        -1,
                        -1,
                        model.RoleLimitTrainers
                    )
                )
                .MustHaveHappenedOnceExactly();
            result.Should().BeRedirectToActionResult().WithActionName("ManageCentre").WithRouteValue("centreId", model.CentreId);
        }
    }
}

