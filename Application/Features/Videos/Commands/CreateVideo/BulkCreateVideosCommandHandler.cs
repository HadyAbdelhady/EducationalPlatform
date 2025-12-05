using Application.DTOs.Section;
using Application.DTOs.Videos;
using Application.Interfaces;
using Application.ResultWrapper;
using CloudinaryDotNet;
using Domain.Entities;
using Domain.enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Videos.Commands.CreateVideo
{
    public class BulkCreateVideosCommandHandler : IRequestHandler<BulkCreateVideosCommand, Result<List<VideoCreationResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BulkCreateVideosCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<List<VideoCreationResponse>>> Handle(BulkCreateVideosCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var videoRepo = _unitOfWork.Repository<Video>();
                var sectionRepo = _unitOfWork.Repository<Domain.Entities.Section>();

                List<Video> videosTobeAdded = new List<Video>();
                var responses = new List<VideoCreationResponse>();

                var Result = 0;

                foreach (var video in request.Videos)
                {
                    var newVideo = new Video()
                    {
                        Id = Guid.NewGuid(),
                        Name = video.Name,
                        Description = video.Description,
                        VideoUrl = video.VideoUrl,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        SectionId = video.SectionId,

                    };
                    videosTobeAdded.Add(newVideo);

                }
                await videoRepo.AddRangeAsync(videosTobeAdded, cancellationToken);

                foreach (var video in videosTobeAdded)
                {
                    responses.Add(new VideoCreationResponse()
                    {
                        VideoId = video.Id,
                        Name = video.Name,
                        CreatedAt = video.CreatedAt.UtcDateTime,
                        Description = video.Description,
                        VideoUrl = video.VideoUrl,

                    });

                    var section = await sectionRepo.GetByIdAsync(video.SectionId.Value );
                    if(section is not null)
                    {
                        section.NumberOfVideos++;
                        section.UpdatedAt = DateTimeOffset.UtcNow;
                        sectionRepo.Update(section);
                    }
                }

               Result =  await _unitOfWork.SaveChangesAsync(cancellationToken);
                if(Result > 0)
                {
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return Result<List<VideoCreationResponse>>.Success(responses);

                }
               
                 await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                return Result<List<VideoCreationResponse>>.FailureStatusCode("Error While Inserting Videos",ErrorType.BadRequest);




            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<List<VideoCreationResponse>>
                    .FailureStatusCode($"Error in bulk create for videos: {ex.Message}", ErrorType.InternalServerError);
            }

        }
    }
}
