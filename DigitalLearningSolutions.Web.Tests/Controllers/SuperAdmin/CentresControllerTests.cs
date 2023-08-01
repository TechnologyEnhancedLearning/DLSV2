using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Tests.TestHelpers;
using DigitalLearningSolutions.Web.Controllers.SuperAdmin.Centres;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.Tests.ControllerHelpers;
using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
using DocumentFormat.OpenXml.EMMA;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.AspNetCore.Mvc;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;

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
            A.CallTo(() => centresDataService.GetContractInfo(CenterId)).Returns(CentreContractAdminUsageTestHelper.GetDefaultEditContractInfo(CenterId));

            // When
            var result = controller.EditContractInfo(centreId);

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
                ContractReviewDate = DateTime.Parse("2023-08-28 16:28:55.247")
            };

            // When
            var result = controller.EditContractInfo(model);

            // Then

            A.CallTo(() => centresDataService.UpdateContractTypeandCenter(model.CentreId,
               model.ContractTypeID,
               model.DelegateUploadSpace,
               model.ServerSpaceBytesInc,
               model.ContractReviewDate
               )).MustHaveHappened();
            result.Should().BeRedirectToActionResult().WithActionName("ManageCentre");
        }
    }
}

