namespace DigitalLearningSolutions.Web.Tests.Models.Enums
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Models.Enums;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class DlsSubApplicationTests
    {
        private static IEnumerable<TestCaseData> ExpectedDlsSubApplicationTestsData
        {
            get
            {
                yield return new TestCaseData(
                    DlsSubApplication.TrackingSystem,
                    0,
                    "TrackingSystem",
                    "Tracking System",
                    "/TrackingSystem/Centre/Dashboard",
                    "Tracking System",
                    "TrackingSystem",
                    0,
                    false,
                    false
                );

                yield return new TestCaseData(
                    DlsSubApplication.Frameworks,
                    1,
                    "Frameworks",
                    "Frameworks",
                    null,
                    null,
                    "Frameworks",
                    3,
                    true,
                    true
                );

                yield return new TestCaseData(
                    DlsSubApplication.Main,
                    2,
                    "Main",
                    "",
                    null,
                    null,
                    null,
                    null,
                    true,
                    true
                );

                yield return new TestCaseData(
                    DlsSubApplication.LearningPortal,
                    3,
                    "LearningPortal",
                    "Learning Portal",
                    "/LearningPortal/Current",
                    "My Current Activities",
                    "LearningPortal",
                    null,
                    true,
                    false
                );

                yield return new TestCaseData(
                    DlsSubApplication.Supervisor,
                    4,
                    "Supervisor",
                    "Supervisor",
                    "/Supervisor",
                    "Supervisor Dashboard",
                    "Supervisor",
                    null,
                    true,
                    false
                );

                yield return new TestCaseData(
                    DlsSubApplication.SuperAdmin,
                    5,
                    "SuperAdmin",
                    "Super Admin",
                    "/SuperAdmin/Admins",
                    "Super Admin - System Configuration",
                    "SuperAdmin",
                    null,
                    false,
                    false
                );
            }
        }

        [Test]
        [TestCaseSource(
            typeof(DlsSubApplicationTests),
            nameof(ExpectedDlsSubApplicationTestsData)
        )]
        public void DlsSubApplication_enums_return_expected_values(
            DlsSubApplication dlsSubApplication,
            int id,
            string name,
            string headerExtension,
            string? headerPath,
            string? headerPathName,
            string? urlSegment,
            int? faqTargetGroupId,
            bool displayHelpMenuItem,
            bool hasNullHeaderData
        )
        {
            using (new AssertionScope())
            {
                dlsSubApplication.Id.Should().Be(id);
                dlsSubApplication.Name.Should().Be(name);
                dlsSubApplication.HeaderExtension.Should().Be(headerExtension);
                dlsSubApplication.UrlSegment.Should().Be(urlSegment);
                dlsSubApplication.FaqTargetGroupId.Should().Be(faqTargetGroupId);
                dlsSubApplication.DisplayHelpMenuItem.Should().Be(displayHelpMenuItem);

                if (hasNullHeaderData)
                {
                    dlsSubApplication.HeaderPath.Should().BeNull();
                    dlsSubApplication.HeaderPathName.Should().BeNull();
                }
                else
                {
                    dlsSubApplication.HeaderPath.Should().EndWith(headerPath);
                    dlsSubApplication.HeaderPathName.Should().EndWith(headerPathName);
                }
            }
        }
    }
}
