using Application.DTOs.Section;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Section.Query.GetSectionsForCourse
{
    public class GetSectionsForCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionsForCourseQuery, Result<List<GetSectionResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<GetSectionResponse>>> Handle(
            GetSectionsForCourseQuery request,
            CancellationToken cancellationToken)
        {
            //var sections = await _unitOfWork.Repository<Domain.Entities.Section>().GetAllAsync(cancellationToken, s => s.CourseId == request.CourseId);
            var sections = await _unitOfWork.Repository<Domain.Entities.Section>().FindAsync(s => s.CourseId == request.CourseId, cancellationToken);


            if (sections == null || !sections.Any())
                return Result<List<GetSectionResponse>>
                    .FailureStatusCode("No sections found for this course", ErrorType.NotFound);

            var response = sections.Select(s => new GetSectionResponse
            {
                SectionId = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CreatedAt = s.CreatedAt
            }).ToList();

            return Result<List<GetSectionResponse>>.Success(response);
        }
    }
}
