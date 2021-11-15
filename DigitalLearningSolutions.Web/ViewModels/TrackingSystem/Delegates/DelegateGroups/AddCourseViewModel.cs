﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AddCourseViewModel : AddCourseFormData
    {
        public AddCourseViewModel(
            int groupId,
            int customisationId,
            IEnumerable<AdminUser> supervisors,
            string groupName,
            CourseDetails courseDetails
        ) : base(groupName, courseDetails)
        {
            GroupId = groupId;
            CustomisationId = customisationId;
            Supervisors = PopulateSupervisors(supervisors);
        }

        public AddCourseViewModel(
            AddCourseFormData formData,
            int groupId,
            int customisationId,
            IEnumerable<AdminUser> supervisors
        ) : base(formData)
        {
            GroupId = groupId;
            CustomisationId = customisationId;
            Supervisors = PopulateSupervisors(supervisors, formData.SupervisorId);
        }

        public int GroupId { get; set; }
        public int CustomisationId { get; set; }
        public IEnumerable<SelectListItem> Supervisors { get; set; }

        private static IEnumerable<SelectListItem> PopulateSupervisors(IEnumerable<AdminUser> supervisors, int? supervisorId = null)
        {
            var supervisorIdNames = supervisors.Select(s => (s.Id, s.FullName));
            return SelectListHelper.MapOptionsToSelectListItems(
                supervisorIdNames,
                supervisorId
            );
        }
    }
}
