using Application.DTOs.Videos;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Videos.Queries.GetAllVideos
{
    public class GetAllVideosQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllVideosQuery, Result<PaginatedResult<VideoByUserIdResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<VideoByUserIdResponse>>> Handle(GetAllVideosQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var videos = await _unitOfWork.Repository<Video>().GetAllAsync(cancellationToken);

                var response = videos.Select(v => new VideoByUserIdResponse()
                {
                    Id = v.Id,
                    Name = v.Name,
                    VideoUrl = v.VideoUrl,
                    //DateOfCreation = v.DateOfCreation,
                    Description = v.Description,
                    NumberOfSheets = v.VideoSheets?.Count ?? 0,
                    SectionId = v.SectionId,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                }).ToList();

                return Result<PaginatedResult<VideoByUserIdResponse>>.Success(new PaginatedResult<VideoByUserIdResponse>
                {
                    Items = response,
                    PageNumber = 1,
                    PageSize = response.Count,
                    TotalCount = response.Count
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<PaginatedResult<VideoByUserIdResponse>>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<VideoByUserIdResponse>>.FailureStatusCode($"An error occurred while retrieving videos: {ex.Message}", ErrorType.InternalServerError);
            }
        }
    }
}
