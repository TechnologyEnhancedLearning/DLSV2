using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Helpers
{
    public class SignpostingHelper
    {
        public static string DisplayTagColour(string resourceType)
        {
            var colours = new string[] { "white", "grey", "green", "aqua-green", "blue", "purple", "red", "orange", "yellow" };
            long hash = resourceType.ToCharArray().Sum(c => c) % colours.Length;
            return colours[hash];
        }
    }
}
