using Application.DTOs.Videos;
using Application.Features.Videos.Commands.CreateVideo;
using Application.Features.Videos.Commands.DeleteVideo;
using Application.Features.Videos.Commands.MarkVideoWatched;
using Application.Features.Videos.Commands.UpdateVideo;
using Application.Features.Videos.Queries.GetAllVideos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController(IMediator mediator, ILogger<VideoController> logger) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;

        [HttpPost("Create")]
        public async Task<IActionResult> CreateVideo(VideoCreationRequest videoCreationRequest, CancellationToken cancellationToken)
        {
            if (videoCreationRequest == null)
                return BadRequest("Creation Request Of Video Must Be Send");

            var createVideoCommand = new CreateVideoCommand()
            {
                Name = videoCreationRequest.Name,
                SectionId = videoCreationRequest.SectionId,
                Description = videoCreationRequest.Description,
                VideoUrl = videoCreationRequest.VideoUrl,
                //DateOfCreation = videoCreationRequest.DateOfCreation,

            };
            var result = await _mediator.Send(createVideoCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);

        }

        [HttpPost("BulkCreateVideos")]
        public async Task<IActionResult> BulkCreateVideo(BulkCreateVideosRequest bulkCreateVideosRequest, CancellationToken cancellationToken)
        {
            if (bulkCreateVideosRequest is null)
                return BadRequest("Creation Request Of Videos Must Be Send");

            var bulkCreateVideosCommand = new BulkCreateVideosCommand
            {
                SectionId = bulkCreateVideosRequest.SectionId,
                Videos = bulkCreateVideosRequest.Videos
            };

            var result = await _mediator.Send(bulkCreateVideosCommand, cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);


        }

        [HttpPatch("Update")]
        public async Task<IActionResult> UpdateVideo(VideoUpdateRequest videoUpdateRequest, CancellationToken cancellationToken)
        {
            if (videoUpdateRequest == null)
                return BadRequest("Update Request Of Video Must Be Send");

            var UpdateVideoCommand = new UpdateVideoCommand()
            {
                VideoId = videoUpdateRequest.VideoId,
                Name = videoUpdateRequest.Name,
                SectionId = videoUpdateRequest.SectionId,
                Description = videoUpdateRequest.Description,
                VideoUrl = videoUpdateRequest.VideoUrl,


            };
            var result = await _mediator.Send(UpdateVideoCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);

        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteVideo(Guid videoId, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Deleting video with Id: {videoId}");
            var command = new DeleteVideoCommand
            {
                VideoId = videoId
            };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideos(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Reterving All Videos");
            var query = new GetAllVideosQuery();

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);

        }

        [HttpPatch("MarkVideoWatched/{VideoId}")]
        public async Task<IActionResult> MarkVideoWatched(Guid VideoId, CancellationToken cancellationToken)
        {
            //var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);
            Guid UserId = Guid.Parse("d446bb09-477d-4c9e-b6fe-6971e6c80dc5");

            var command = new VideoWatchedCommand(VideoId, UserId);

            var result = await _mediator.Send(command, cancellationToken);
            return result ? Ok() : StatusCode(500, "Failed to mark video as watched.");
        }

    }
}
