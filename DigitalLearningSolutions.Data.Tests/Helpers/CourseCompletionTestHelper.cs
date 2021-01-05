namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using Microsoft.Data.SqlClient;

    public class CourseCompletionTestHelper
    {
        private SqlConnection connection;

        public CourseCompletionTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void AddCertificationToCourse(int customisationId)
        {
            connection.Execute(
                @"UPDATE Applications
                     SET IncludeCertification = 1
                    FROM Customisations

                   INNER JOIN Applications
                      ON Applications.ApplicationID = Customisations.ApplicationID

                   WHERE Customisations.CustomisationID = @customisationId;",
                new { customisationId }
            );
        }
    }
}
