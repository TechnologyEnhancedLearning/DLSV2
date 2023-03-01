using DigitalLearningSolutions.Data.Models.Certificates;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    public static class CertificateTestHelper
    {
        public static CertificateInformation GetDefaultCertificate
        (
               int progressID = 0,
                string? delegateFirstName = "Joseph",
               string delegateLastName = "Bloggs",
               string? contactForename = "xxxxx",
               string? contactSurname = "xxxx",
               string centreName = "North West Boroughs Healthcare NHS Foundation Trust",
               int centreID = 2,
               byte[]? signatureImage = null,
               int signatureWidth = 250,
               int signatureHeight = 250,
               byte[]? centreLogo = null,
               int logoWidth = 250,
               int logoHeight = 250,
               string? logoMimeType = null,
               string courseName = "Level 2 - ITSP Course Name",
               DateTime? completionDate = null,
               int appGroupID = 3,
               int createdByCentreID = 2
              )
        {
            return new CertificateInformation
            {
                ProgressID = progressID,
                DelegateFirstName = delegateFirstName,
                DelegateLastName = delegateLastName,
                ContactForename = contactForename,
                ContactSurname = contactSurname,
                CentreName = centreName,
                CentreID = centreID,
                SignatureImage = signatureImage,
                SignatureWidth = signatureWidth,
                SignatureHeight = signatureHeight,
                CentreLogo = centreLogo,
                LogoWidth = logoWidth,
                LogoHeight = logoHeight,
                LogoMimeType = logoMimeType,
                CourseName = courseName,
                CompletionDate = DateTime.Parse("2023-02-27 16:28:55.247"),
                AppGroupID = appGroupID,
                CreatedByCentreID = createdByCentreID,
            };
        }




    }
}
