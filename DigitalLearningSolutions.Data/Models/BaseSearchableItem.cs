﻿namespace DigitalLearningSolutions.Data.Models
{
    /// <summary>
    /// Base class for classes to implement to allow searching of a list of that class
    /// </summary>
    public abstract class BaseSearchableItem
    {
        /// <summary>
        /// This property exists as we need to set the search string on an instance of
        /// the type for the query for the FuzzySharp search parameters.
        /// See GenericSearchHelper.cs in the DigitalLearningSolutions.Web project
        /// </summary>
        public string? SearchableNameOverrideForFuzzySharp;

        public abstract string SearchableName { get; set; }
    }
}
