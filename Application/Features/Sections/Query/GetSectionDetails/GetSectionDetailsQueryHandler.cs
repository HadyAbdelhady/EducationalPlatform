using Application.DTOs.Sections;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.enums;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionDetails
{
    public class GetSectionDetailsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionDetailsQuery, Result<GetSectionDetailsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<GetSectionDetailsResponse>> Handle(GetSectionDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _unitOfWork.GetRepository<ISectionRepository>()
                                                                     .GetSectionDetailsResponse(request, cancellationToken);

                var response = new GetSectionDetailsResponse
                {
                    SectionId = data.Section.Id,
                    Name = data.Section.Name,
                    Description = data.Section.Description,
                    Price = data.Section.Price,
                    Rating = data.Section.Rating,
                    CreatedAt = data.Section.CreatedAt,
                    UpdatedAt = data.Section.UpdatedAt ?? data.Section.CreatedAt,
                    CourseId = data.Section.CourseId ?? Guid.Empty,
                    NumberOfVideos = data.Videos.Count,
                    NumberOfQuestionSheets = data.Section.NumberOfQuestionSheets,
                    NumberOfSectionVideosWatched = data.StudentSection?.NumberOfSectionVideosWatched ?? 0,

                    Videos = [.. data.Videos.Select(v => new VideoData
                    {
                        Id = v.Id,
                        Name = v.Name,
                        VideoUrl = v.VideoUrl,
                        Rating = v.Rating,
                        StudentVideo = data.StudentSection is null
                            ? null
                            : new StudentVideoData
                            {
                                WatchedAt = v.StudentVideo?.WatchedAt,
                                Progress = v.StudentVideo?.Progress ?? 0
                            }
                    })]
                };

                return Result<GetSectionDetailsResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<GetSectionDetailsResponse>.FailureStatusCode(
                    $"An error occurred while retrieving section details: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}
