using Application.DTOs.Sections;
using Application.DTOs.Videos;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionsForCourse
{
    public class GetSectionsForCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionsForCourseQuery, Result<List<GetSectionResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<GetSectionResponse>>> Handle(
            GetSectionsForCourseQuery request,
            CancellationToken cancellationToken)
        {
            var sections = await _unitOfWork.Repository<Section>()
                                                             .FindAsync(s => s.CourseId == request.CourseId, cancellationToken,v=>v.Videos);

            if (sections == null || !sections.Any())
                return Result<List<GetSectionResponse>>
                    .FailureStatusCode("No sections found for this course", ErrorType.NotFound);

            var response = sections.Select(s => new GetSectionResponse
            {
                SectionId = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                NumberOfQuestionSheets = s.NumberOfQuestionSheets,
                CreatedAt = s.CreatedAt,
                VideoInfo = [.. s.Videos.Select(v => new VideoResponse
                {
                    VideoId = v.Id,
                    Name = v.Name,
                    VideoUrl = v.VideoUrl,
                    CreatedAt = v.CreatedAt,
                    Description = v.Description
                })]
            }).ToList();

            return Result<List<GetSectionResponse>>.Success(response);
        }
    }
}
