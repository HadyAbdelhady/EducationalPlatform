namespace Domain.enums
{
    public enum EnrollmentProgressType
    {
        Course,
        SectionOnly
    }

    public static class EnrollmentProgressTypeExtensions
    {
        public static string ToApiValue(this EnrollmentProgressType type) =>
            type switch
            {
                EnrollmentProgressType.Course => "Course",
                EnrollmentProgressType.SectionOnly => "SectionOnly",
                _ => type.ToString()
            };

        public static EnrollmentProgressType FromApiValue(string value) =>
            value switch
            {
                "SectionOnly" => EnrollmentProgressType.SectionOnly,
                _ => EnrollmentProgressType.Course
            };
    }
}
