using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Centres;
using DigitalLearningSolutions.Data.Tests.TestHelpers;
using DigitalLearningSolutions.Web.Controllers.SuperAdmin.Centres;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Tests.Controllers.SuperAdmin
{
    public class CentresControllerTests
    {
        private const int CenterId = 374;
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
            IEnumerable<(int, string)> centresList = new List<(int, string)> { (374, "##HEE Demo Centre1##") };
            A.CallTo(() => centresDataService.GetAllCentres(false)).Returns(centresList);
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
        public void EditCentreDetails_results_DuplicateCentre_error()
        {
            // Given
            IEnumerable<(int, string)> centresList = new List<(int, string)> { (374, "##HEE Demo Centre##") };
            A.CallTo(() => centresDataService.GetAllCentres(false)).Returns(centresList);
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
            result.Should().BeViewResult();
            controller.ModelState.IsValid.Should().BeFalse();
            var centreNameErrors = controller.ModelState["CentreName"].Errors;
            centreNameErrors.Should().NotBeEmpty();
            centreNameErrors.Should().Contain(error => error.ErrorMessage ==
            "The centre name you have entered already exists, please enter a different centre name");

            A.CallTo(() => centresDataService.UpdateCentreDetailsForSuperAdmin(
                                                model.CentreId,
                                                model.CentreName,
                                                model.CentreTypeId,
                                                model.RegionId,
                                                model.CentreEmail,
                                                model.IpPrefix,
                                                model.ShowOnMap))
                                                .MustNotHaveHappened();
        }

        [Test]
        public void AddCentre_adds_centre_and_redirects_with_successful_save()
        {
            // Given
            var model = new AddCentreSuperAdminViewModel
            {
                CentreName = "##HEE Demo Centre##",
                ContactFirstName = "FirstName",
                ContactLastName = "LastName",
                ContactEmail = "sample@email.com",
                ContactPhone = "07384562856",
                CentreTypeId = 1,
                RegionId = 3,
                RegistrationEmail = "sample2@email.com",
                IpPrefix = "192.164.1.1",
                ShowOnMap = true,
                AddITSPcourses = true
            };

            // When
            var result = controller.AddCentre(model);

            // Then
            result.Should().BeRedirectToActionResult().WithActionName("ManageCentre");
            A.CallTo(() => centresDataService.AddCentreForSuperAdmin(
                                                model.CentreName,
                                                model.ContactFirstName,
                                                model.ContactLastName,
                                                model.ContactEmail,
                                                model.ContactPhone,
                                                model.CentreTypeId,
                                                model.RegionId,
                                                model.RegistrationEmail,
                                                model.IpPrefix,
                                                model.ShowOnMap,
                                                model.AddITSPcourses))
                                                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void AddCentre_results_DuplicateCentre_error()
        {
            // Given
            IEnumerable<(int, string)> centresList = new List<(int, string)> { (374, "##HEE Demo Centre##") };
            A.CallTo(() => centresDataService.GetAllCentres(false)).Returns(centresList);
            var model = new AddCentreSuperAdminViewModel
            {
                CentreName = "##HEE Demo Centre##",
                ContactFirstName = "FirstName",
                ContactLastName = "LastName",
                ContactEmail = "sample@email.com",
                ContactPhone = "07384562856",
                CentreTypeId = 1,
                RegionId = 3,
                RegistrationEmail = "sample2@email.com",
                IpPrefix = "192.164.1.1",
                ShowOnMap = true,
                AddITSPcourses = true
            };

            // When
            var result = controller.AddCentre(model);

            // Then
            result.Should().BeViewResult();

            controller.ModelState.IsValid.Should().BeFalse();
            var centreNameErrors = controller.ModelState["CentreName"].Errors;
            centreNameErrors.Should().NotBeEmpty();
            centreNameErrors.Should().Contain(error => error.ErrorMessage ==
            "The centre name you have entered already exists, please enter a different centre name");

            A.CallTo(() => centresDataService.AddCentreForSuperAdmin(
                                                model.CentreName,
                                                model.ContactFirstName,
                                                model.ContactLastName,
                                                model.ContactEmail,
                                                model.ContactPhone,
                                                model.CentreTypeId,
                                                model.RegionId,
                                                model.RegistrationEmail,
                                                model.IpPrefix,
                                                model.ShowOnMap,
                                                model.AddITSPcourses)).MustNotHaveHappened();
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
                RoleLimitCcLicences = 10,           // set
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
                IsRoleLimitSetContentCreatorLicences = true,
                RoleLimitContentCreatorLicences = 10,
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
                IsRoleLimitSetContentCreatorLicences = true,
                IsRoleLimitSetCustomCourses = false,
                IsRoleLimitSetTrainers = true,
                RoleLimitCmsAdministrators = 1,
                RoleLimitCmsManagers = -1,
                RoleLimitContentCreatorLicences = 2,
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
                        model.RoleLimitContentCreatorLicences,
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
                IsRoleLimitSetContentCreatorLicences = false,
                IsRoleLimitSetCustomCourses = false,
                IsRoleLimitSetTrainers = true,
                RoleLimitCmsAdministrators = 1,
                RoleLimitCmsManagers = 10,
                RoleLimitContentCreatorLicences = 20,
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
        [Test]
        public void Get_with_centreId_shows_EditContractInfo_page()
        {
            // Given
            const int centreId = 374;
            const string centreName = "##HEE Demo Centre##";
            const string contractType = "Premium";
            const int contractTypeID = 1;
            const long serverSpaceBytesInc = 5368709120;
            const long delegateUploadSpace = 52428800;
            DateTime contractReviewDate = DateTime.Parse("2023-08-28 16:28:55.247");
            const int contractReviewDay = 28;
            const int contractReviewMonth = 8;
            const int contractReviewYear = 2023;
            A.CallTo(() => centresDataService.GetContractInfo(CenterId)).Returns(CentreContractAdminUsageTestHelper.GetDefaultEditContractInfo(CenterId));

            // When
            var result = controller.EditContractInfo(centreId,28,8,2023,10024,10024,100024);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().CentreId.Should().Be(centreId);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().CentreName.Should().Be(centreName);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().ContractType.Should().Be(contractType);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().ContractTypeID.Should().Be(contractTypeID);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().ServerSpaceBytesInc.Should().Be(serverSpaceBytesInc);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().DelegateUploadSpace.Should().Be(delegateUploadSpace);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().ContractReviewDate.Should().Be(contractReviewDate);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().ContractReviewDay.Should().Be(contractReviewDay);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().ContractReviewMonth.Should().Be(contractReviewMonth);
                result.Should().BeViewResult().ModelAs<ContractTypeViewModel>().ContractReviewYear.Should().Be(contractReviewYear);
            }
        }

        [Test]
        public void Edit_ContractInfo_redirects_with_successful_save()
        {
            // Given
            var model = new ContractTypeViewModel
            {
                CentreId = 374,
                CentreName = "##HEE Demo Centre##",
                ContractType = "Basic",
                ContractTypeID = 1,
                ServerSpaceBytesInc = 5368709120,
                DelegateUploadSpace = 52428800,
                ContractReviewDate = DateTime.Parse(DateTime.UtcNow.ToString()),
                ContractReviewDay = DateTime.UtcNow.Day,
                ContractReviewMonth = DateTime.UtcNow.Month,
                ContractReviewYear = DateTime.UtcNow.Year
            };
            DateTime date = new DateTime(model.ContractReviewYear.Value, model.ContractReviewMonth.Value, model.ContractReviewDay.Value, 0, 0, 0);

            // When
            var result = controller.EditContractInfo(model, DateTime.UtcNow.Day, DateTime.UtcNow.Month, DateTime.UtcNow.Year);
            A.CallTo(() => centresDataService.UpdateContractTypeandCenter(model.CentreId,
               model.ContractTypeID,
               model.DelegateUploadSpace,
               model.ServerSpaceBytesInc,
               date
               )).MustHaveHappened();
            // Then
            result.Should().BeRedirectToActionResult().WithActionName("ManageCentre");
        }
    }
}

