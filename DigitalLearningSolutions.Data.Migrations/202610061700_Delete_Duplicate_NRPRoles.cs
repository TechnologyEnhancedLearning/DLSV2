using FluentMigrator;

namespace DigitalLearningSolutions.Data.Migrations
{
    [Migration(202610061700)]
    public class Delete_Duplicate_NRPRoles : Migration
    {
        public override void Up()
        {
            Execute.Sql("DELETE FROM NRPRoles WHERE ID IN (214,221,339,362)");
            Execute.Sql(@"
                UPDATE NRPRoles
                    SET RoleProfile = CASE ID
                        WHEN 4 THEN 'Business/Administrative Manager'
                        WHEN 5 THEN 'Business/Administrative Manager Higher Level'
                        WHEN 8 THEN 'Commissioning Manager'

                        WHEN 45 THEN 'Information Management and Technology Operator/Telephony Operator'
                        WHEN 46 THEN 'Information Management and Technology Analyst/Technician Entry Level'
                        WHEN 47 THEN 'Information Management and Technology Operator Team Leader/Telephony Operator Team Leader'
                        WHEN 48 THEN 'Information Management and Technology Analyst/Technician'
                        WHEN 51 THEN 'Information Management and Technology Analyst/Technician Higher Level'
                        WHEN 53 THEN 'Information Management and Technology Analyst Specialist/Technical Engineer/Team Leader'
                        WHEN 54 THEN 'Information Management and Technology Analyst Advanced/Technical Engineer Specialist'
                        WHEN 55 THEN 'Information Management and Technology Section Manager'
                        WHEN 58 THEN 'Information Management and Technology Consultant'
                        WHEN 59 THEN 'Information Management and Technology Service Manager'

                        WHEN 74 THEN 'Communications Specialist'

                        WHEN 118 THEN 'Clinical Psychologist Consultant'

                        WHEN 204 THEN 'Specialist Speech and Language Therapist'

                        WHEN 280 THEN 'Healthcare Scientist Advanced'
                        WHEN 283 THEN 'Physician Associate Entry Level'

                        WHEN 335 THEN 'Dental Nurse Higher Level'

                        WHEN 340 THEN 'Practice Education Facilitator'

                    END
                WHERE ID IN (4, 5, 8, 45, 46, 47, 48, 51, 53, 54, 55, 58, 59, 74, 118, 204, 280, 283, 335, 340);
                ");

        }
        public override void Down()
        {
            Execute.Sql(@"
                UPDATE NRPRoles
                    SET RoleProfile = CASE ID
                        WHEN 4 THEN 'Business/Administative Manager'
                        WHEN 5 THEN 'Business/Administative Manager Higher Level'
                        WHEN 8 THEN 'Commisssioning Manager'

                        WHEN 45 THEN 'Information Management and Technnology Operator/Telephony Operator'
                        WHEN 46 THEN 'Information Management and Technnology Analyst/Technician Entry Level'
                        WHEN 47 THEN 'Information Management and Technnology Operator Team Leader/Telephony Operator Team Leader'
                        WHEN 48 THEN 'Information Management and Technnology Analyst/Technician'
                        WHEN 51 THEN 'Information Management and Technnology Analyst/Technician Higher Level'
                        WHEN 53 THEN 'Information Management and Technnology Analyst Specialist/Technical Engineer/Team Leader'
                        WHEN 54 THEN 'Information Management and Technnology Analyst Advanced//Technicial Engineer Specialist'
                        WHEN 55 THEN 'Information Management and Technnology Section Manager'
                        WHEN 58 THEN 'Information Management and Technnology Consultant'
                        WHEN 59 THEN 'Information Management and Technnology Service Manager'

                        WHEN 74 THEN 'Communicatons Specialist'

                        WHEN 118 THEN 'Clinical Pyschologist Consultant'

                        WHEN 204 THEN 'Specialist Speech and Language Therapist '

                        WHEN 280 THEN 'Healthcare Scientist Advanced '
                        WHEN 283 THEN 'Physican Assocate Entry Level '

                        WHEN 335 THEN 'Dental Nurse Higher Level '

                        WHEN 340 THEN 'Practic Education Facilitator'

                    END
                WHERE ID IN (4, 5, 8, 45, 46, 47, 48, 51, 53, 54, 55, 58, 59, 74, 118, 204, 280, 283, 335, 340);
                ");
        }
    }
}
