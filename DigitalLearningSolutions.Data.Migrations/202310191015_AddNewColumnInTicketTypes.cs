using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Data.Migrations
{
    using FluentMigrator;
    [Migration(202310191018)]
    public class AddNewColumnInTicketTypes : Migration
    {
        public override void Up()
        {
            Execute.Sql(
                   @$"BEGIN
                        ALTER TABLE [TicketTypes] ADD FreshdeskTicketType nvarchar(50);                        
                    END"
               );
            Execute.Sql(
                @$"BEGIN
                        UPDATE [TicketTypes] SET FreshdeskTicketType = 'Question' WHERE [TicketTypeID] = 1;
                        UPDATE [TicketTypes] SET FreshdeskTicketType = 'Question' WHERE [TicketTypeID] = 2;
                        UPDATE [TicketTypes] SET FreshdeskTicketType = 'Feature Request' WHERE [TicketTypeID] = 3;
                        UPDATE [TicketTypes] SET FreshdeskTicketType = 'Problem' WHERE [TicketTypeID] = 4;
                    END"
            );
        }

        public override void Down()
        {
            Execute.Sql(@"ALTER TABLE [TicketTypes] DROP COLUMN FreshdeskTicketType;");
        }
    }
}
