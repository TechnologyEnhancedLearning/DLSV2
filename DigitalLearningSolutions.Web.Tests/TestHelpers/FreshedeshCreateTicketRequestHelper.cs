using DigitalLearningSolutions.Data.Models.Email;
using DigitalLearningSolutions.Web.Models;
using DigitalLearningSolutions.Web.Models.Enums;
using DocumentFormat.OpenXml.Bibliography;
using FreshdeskApi.Client.CommonModels;
using FreshdeskApi.Client.Tickets.Models;
using FreshdeskApi.Client.Tickets.Requests;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    public static class FreshedeshCreateTicketRequestHelper
    {
        public static RequestSupportTicketData FreshedeshCreateNewTicket(string? userName = "TestUser",
            string? userCentreEmail = null, int? adminUserID = 1, string? centreName = "Test Centre",
            string? requestDescription = "New Ticket ", string? freshdeskRequestType = null, string? requestSubject = "Please Check"
        )
        {
            return new RequestSupportTicketData
            {
                CentreName = centreName,
                AdminUserID = adminUserID,
                UserCentreEmail = userCentreEmail,
                UserName = userName,
                RequestDescription = requestDescription,
                FreshdeskRequestType = freshdeskRequestType,
                RequestSubject = requestSubject
            };
        }
    }
}
