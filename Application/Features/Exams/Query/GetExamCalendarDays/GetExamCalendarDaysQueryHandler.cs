using Application.DTOs.Exam;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Exams.Query.GetExamCalendarDays
{
    public class GetExamCalendarDaysQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetExamCalendarDaysQuery, Result<List<ExamCalendarItemDto>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<ExamCalendarItemDto>>> Handle(GetExamCalendarDaysQuery request, CancellationToken cancellationToken)
        {
            var examsRepo = _unitOfWork.Repository<Exam>().GetAll(cancellationToken);
            var coursesRepo = _unitOfWork.Repository<Course>().GetAll(cancellationToken);
            var sectionsRepo = _unitOfWork.Repository<Section>().GetAll(cancellationToken);

          
            var examsFromCourse =
                from e in examsRepo
                join c in coursesRepo on e.CourseId equals c.Id
                where c.EducationYearId == request.EducationYearId
                      && e.InstructorId == request.InstructorId
                      && e.StartTime.HasValue
                select e;

            var examsFromSection =
                from e in examsRepo
                join s in sectionsRepo on e.SectionId equals s.Id
                join c in coursesRepo on s.CourseId equals c.Id
                where  c.EducationYearId == request.EducationYearId
                      && e.InstructorId == request.InstructorId
                      && e.StartTime.HasValue
                select e;

            var examsQuery = examsFromCourse.Union(examsFromSection);

            if (request.SectionId.HasValue)
            {
                var sectionId = request.SectionId.Value;
                examsQuery = examsQuery.Where(e => e.SectionId == sectionId);
            }

            if (request.CourseId.HasValue)
            {
                var courseId = request.CourseId.Value;

                examsQuery = examsQuery.Where(e =>
                    e.CourseId == courseId ||
                    (e.SectionId.HasValue &&
                     sectionsRepo.Any(s => s.Id == e.SectionId.Value && s.CourseId == courseId)));
            }

            var items = examsQuery
                .Select(e => new
                {
                    Date = e.StartTime!.Value.Date,
                    Type = e.SectionId.HasValue ? "Section" : "Course",
                    ExamName = e.Name
                })
                .ToList()
                .Select(x => new ExamCalendarItemDto
                {
                    Date = x.Date.ToString("yyyy-MM-dd"),
                    Type = x.Type,
                    ExamName = x.ExamName
                })
                .OrderBy(i => i.Date)
                .ThenBy(i => i.Type)
                .ThenBy(i => i.ExamName)
                .ToList();

            return await Task.FromResult(Result<List<ExamCalendarItemDto>>.Success(items));
        }
    }
}
