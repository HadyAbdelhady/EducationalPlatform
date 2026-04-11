namespace Application.DTOs.Exam
{
    public class GetStudentsSubmittionsForExamRequest
    {
        public Guid ExamId { get; set; }
        public GetAllEntityRequestSkeleton RequestSkeleton { get; set; } = new GetAllEntityRequestSkeleton();
    }
}
