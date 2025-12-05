using Application.DTOs.Videos;
using Application.Features.Course.Commands.DeleteCourse;
using Application.Features.Videos.Commands.CreateVideo;
using Application.Features.Videos.Commands.DeleteVideo;
using Application.Features.Videos.Commands.UpdateVideo;
using Application.Features.Videos.Queries.GetAllVideos;
using Application.ResultWrapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public VideoController(IMediator mediator , ILogger<VideoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("Create")]

        public async Task<IActionResult> CreateVideo(VideoCreationRequest videoCreationRequest , CancellationToken cancellationToken)
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
            var result = await _mediator.Send(createVideoCommand,cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);

        }

        [HttpPost("BulkCreateVideos")]
        public async Task<IActionResult> BulkCreateVideo(BulkCreateVideosRequest bulkCreateVideosRequest , CancellationToken cancellationToken)
        {
            if (bulkCreateVideosRequest is  null)
                return BadRequest("Creation Request Of Videos Must Be Send");

            var bulkCreateVideosCommand = new BulkCreateVideosCommand(bulkCreateVideosRequest.Videos);

            var result  = await _mediator.Send(bulkCreateVideosCommand,cancellationToken);

            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);


        }

        [HttpPost("Update")]
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
            return result.IsSuccess ? Ok(result.Value) : StatusCode((int)result.ErrorType, result.Error);

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
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideos(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Reterving All Videos");
            var query = new GetAllVideosQuery();
            
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);

        }

    }
}
