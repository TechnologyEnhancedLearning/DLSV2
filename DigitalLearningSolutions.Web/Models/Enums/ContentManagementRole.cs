namespace DigitalLearningSolutions.Web.Models.Enums
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class ContentManagementRole : Enumeration
    {
        public static readonly ContentManagementRole NoContentManagementRole =
            new ContentManagementRole(0, nameof(NoContentManagementRole), false, false);

        public static readonly ContentManagementRole CmsManager = new ContentManagementRole(
            1,
            nameof(CmsManager),
            true,
            false
        );

        public static readonly ContentManagementRole CmsAdministrator = new ContentManagementRole(
            2,
            nameof(CmsAdministrator),
            true,
            true
        );

        public readonly bool ImportOnly;
        public readonly bool IsContentManager;

        private ContentManagementRole(int id, string name, bool isContentManager, bool importOnly) : base(id, name)
        {
            IsContentManager = isContentManager;
            ImportOnly = importOnly;
        }

        public static implicit operator ContentManagementRole(string value)
        {
            try
            {
                return FromName<ContentManagementRole>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string(ContentManagementRole contentManagementRole)
        {
            return contentManagementRole.Name;
        }
    }
}
