namespace Application.HelperFunctions
{
    public static class GradeMapping
    {
        /// <summary>
        /// Maps a numeric grade (0-100) to a letter grade.
        /// </summary>
        public static string ToLetterGrade(decimal numericGrade)
        {
            return numericGrade switch
            {
                >= 93 => "A",
                >= 90 => "A-",
                >= 87 => "B+",
                >= 83 => "B",
                >= 80 => "B-",
                >= 77 => "C+",
                >= 73 => "C",
                >= 70 => "C-",
                >= 67 => "D+",
                >= 63 => "D",
                >= 60 => "D-",
                _ => "F"
            };
        }
    }
}
