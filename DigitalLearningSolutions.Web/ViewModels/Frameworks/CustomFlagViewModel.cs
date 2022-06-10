using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    public class CustomFlagViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string FlagName { get; set; }

        [Required]
        [StringLength(100)]
        public string FlagGroup { get; set; }

        [Required]
        [StringLength(100)]
        public string FlagTagClass { get; set; }

        public Dictionary<string, string> TagColors = new Dictionary<string, string>();

        public CustomFlagViewModel()
        {
            var colors = new string[] { "White", "Grey", "Green", "Aqua green", "Blue", "Purple", "Pink", "Red", "Orange", "Yellow" };
            TagColors = colors.ToDictionary(c => $"nhsuk-tag--{c.ToLower().Replace(" ", "-")}");
        }
    }
}
