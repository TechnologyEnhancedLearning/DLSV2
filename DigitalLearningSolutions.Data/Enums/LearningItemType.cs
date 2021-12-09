namespace DigitalLearningSolutions.Data.Enums
{
    using System;
    using System.ComponentModel;

    [TypeConverter(typeof(EnumerationTypeConverter<LearningItemType>))]
    public class LearningItemType : Enumeration
    {
        public static readonly LearningItemType Course = new LearningItemType(
            0,
            nameof(Course)
        );

        public static readonly LearningItemType SelfAssessment = new LearningItemType(
            1,
            nameof(SelfAssessment)
        );

        public static readonly LearningItemType Resource = new LearningItemType(
            2,
            nameof(Resource)
        );

        private LearningItemType(int id, string name) : base(id, name) { }

        public static implicit operator LearningItemType(string value)
        {
            try
            {
                return FromName<LearningItemType>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }

        public static implicit operator string(LearningItemType type)
        {
            return type.Name;
        }
    }
}
