namespace DigitalLearningSolutions.Data.Helpers
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Constants;

    /// <summary>
    /// Resolves which active admin account to use for an email address.
    /// An email address may have an admin account at more than one centre,
    /// so the caller must supply the chosen admin ID when there is more than one.
    /// </summary>
    public static class AdminAccountResolver
    {
        /// <returns>
        /// The resolved admin ID when successful (positive), otherwise
        /// <see cref="CollaboratorResultCodes.NoActiveAdminAccount"/> or
        /// <see cref="CollaboratorResultCodes.AdminAccountSelectionRequired"/>.
        /// </returns>
        public static int ResolveActiveAdminId(
            IDbConnection connection,
            string userEmail,
            int? selectedAdminId,
            IDbTransaction? transaction = null
        )
        {
            var activeAdminIds = connection.Query<int>(
                @"SELECT AdminID FROM AdminUsers WHERE Email = @userEmail AND Active = 1",
                new { userEmail },
                transaction
            ).ToList();

            if (activeAdminIds.Count == 0)
            {
                return CollaboratorResultCodes.NoActiveAdminAccount;
            }

            if (selectedAdminId.HasValue)
            {
                // Guard against a tampered or stale selection: the chosen account
                // must be an active admin account belonging to this email address.
                return activeAdminIds.Contains(selectedAdminId.Value)
                    ? selectedAdminId.Value
                    : CollaboratorResultCodes.NoActiveAdminAccount;
            }

            return activeAdminIds.Count == 1
                ? activeAdminIds[0]
                : CollaboratorResultCodes.AdminAccountSelectionRequired;
        }
    }
}
