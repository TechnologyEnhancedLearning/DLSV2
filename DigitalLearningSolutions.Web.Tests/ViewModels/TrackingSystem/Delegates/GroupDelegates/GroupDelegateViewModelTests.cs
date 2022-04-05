namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.GroupDelegates
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class GroupDelegateViewModelTests
    {
        [Test]
        public void GroupDelegateViewModel_populates_expected_values_with_both_names()
        {
            // Given
            var groupDelegate = Builder<GroupDelegate>.CreateNew()
                .With(gd => gd.GroupDelegateId = 62)
                .With(gd => gd.FirstName = "Test")
                .With(gd => gd.LastName = "Name")
                .With(gd => gd.EmailAddress = "gslectik.m@vao")
                .With(gd => gd.CandidateNumber = "KT553")
                .Build();

            // When
            var result = new GroupDelegateViewModel(groupDelegate, new ReturnPageQuery("pageNumber=1"));

            // Then
            using (new AssertionScope())
            {
                result.GroupDelegateId.Should().Be(62);
                result.Name.Should().Be("Test Name");
                result.EmailAddress.Should().Be("gslectik.m@vao");
                result.CandidateNumber.Should().Be("KT553");
            }
        }

        [Test]
        public void GroupDelegateViewModel_populates_expected_values_with_only_last_name()
        {
            // Given
            var groupDelegate = Builder<GroupDelegate>.CreateNew()
                .With(gd => gd.GroupDelegateId = 62)
                .With(gd => gd.FirstName = null)
                .With(gd => gd.LastName = "Name")
                .With(gd => gd.EmailAddress = "gslectik.m@vao")
                .With(gd => gd.CandidateNumber = "KT553")
                .Build();

            // When
            var result = new GroupDelegateViewModel(groupDelegate, new ReturnPageQuery("pageNumber=1"));

            // Then
            using (new AssertionScope())
            {
                result.GroupDelegateId.Should().Be(62);
                result.Name.Should().Be("Name");
                result.EmailAddress.Should().Be("gslectik.m@vao");
                result.CandidateNumber.Should().Be("KT553");
            }
        }
    }
}
