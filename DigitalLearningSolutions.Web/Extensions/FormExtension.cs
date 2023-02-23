using DigitalLearningSolutions.Data.Models.SessionData.SelfAssessments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Extensions
{
    public static class FormExtension
    {
        public static async Task<bool> IsDuplicateSubmission(this HttpContext context)
        {

            var currentToken = context.Request.Form["__RequestVerificationToken"].ToString();
            var lastToken = context.Session.GetString("LastProcessedToken");

            if (lastToken == currentToken)
            {
                return true;
            }
            else
            {
                context.Session.SetString("LastProcessedToken", currentToken);
                await context.Session.CommitAsync();
                return false;
            }
        }
    }
}
