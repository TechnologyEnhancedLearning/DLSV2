﻿namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse;

    public class MultiPageFormDataFeature : Enumeration
    {
        public static readonly MultiPageFormDataFeature AddNewCourse = new MultiPageFormDataFeature(
            0,
            nameof(AddNewCourse),
            "AddNewCourseDataGuid",
            typeof(AddNewCentreCourseData)
        );

        //public static readonly MultiPageFormDataFeature EditRegistrationPrompt = new MultiPageFormDataFeature(
        //    0,
        //    nameof(EditRegistrationPrompt),
        //    "EditRegistrationPromptDataGuid"
        //);

        //public static readonly MultiPageFormDataFeature EditAdminField = new MultiPageFormDataFeature(
        //    0,
        //    nameof(EditAdminField),
        //    "EditAdminFieldDataGuid"
        //);

        public readonly string TempDataKey;

        public readonly Type Type;

        public MultiPageFormDataFeature(int id, string name, string tempDataKey, Type type) : base(id, name)
        {
            TempDataKey = tempDataKey;
            Type = type;
        }

        public static MultiPageFormDataFeature? GetFeatureByType(Type type)
        {
            return GetAll<MultiPageFormDataFeature>().SingleOrDefault(m => m.Type == type);
        }
    }
}
