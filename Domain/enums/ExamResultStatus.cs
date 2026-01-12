namespace Domain.enums
{
    public enum ExamResultStatus
    {
        NotStarted, // exam has not been started by the student
        InProgress,// exam is still being taken by the student
        Passed, // student has passed the exam
        Failed // student has failed the exam
    }
}
