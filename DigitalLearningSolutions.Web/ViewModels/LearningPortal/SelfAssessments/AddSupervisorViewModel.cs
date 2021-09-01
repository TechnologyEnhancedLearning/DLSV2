﻿namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Attributes;
    using System.ComponentModel.DataAnnotations;

    public class AddSupervisorViewModel
    {
        public int SelfAssessmentID { get; set; }
        public string SelfAssessmentName { get; set; }
        [Required(ErrorMessage = "Enter an supervisor email address")]
        [MaxLength(255, ErrorMessage = "Supervisor email address must be 255 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter a supervisor email address in the correct format, like name@example.com")]
        [NoWhitespace("Supervisor email address must not contain any whitespace characters")]
        public string SupervisorEmail { get; set; }
    }
}
