using Application.DTOs;
using Application.DTOs.Videos;
using Application.Features.Videos.Commands.CreateVideo;
using Application.Features.Videos.Commands.DeleteVideo;
using Application.Features.Videos.Commands.MarkVideoWatched;
using Application.Features.Videos.Commands.UpdateVideo;
using Application.Features.Videos.Queries.GetAllVideos;
using Application.Features.Videos.Queries.GetVideoById;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController(IMediator mediator, ILogger<VideoController> logger, ICloudinaryCore cloudinaryService) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ILogger _logger = logger;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        #region Direct Upload Endpoints

        /// <summary>
        /// Step 1 of Direct Upload: Generate a Cloudinary signature.
        /// 
        /// The client calls this BEFORE uploading. We validate file metadata
        /// and return a signature that authorizes a direct upload to Cloudinary.
        /// No file binary is sent to this endpoint - only metadata.
        /// </summary>
        [HttpPost("generate-upload-signature")]
        public async Task<IActionResult> GenerateVideoUploadSignature(
            [FromBody] DirectVideoUploadSignatureRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest("Signature request is required.");

                if (string.IsNullOrWhiteSpace(request.FileName))
                    return BadRequest("FileName is required.");

                if (request.FileSize <= 0)
                    return BadRequest("FileSize must be greater than 0.");

                var signatureResponse = await _cloudinaryService
                    .GenerateDirectUploadSignatureAsync(request);

                return Ok(signatureResponse);
            }
            catch (ArgumentException ex)
            {
                // Validation failures (wrong file type, too large, etc.)
                _logger.LogWarning("Signature generation rejected: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate upload signature");
                return StatusCode(500, new { error = "An unexpected error occurred while generating the upload signature." });
            }
        }


        /// <summary>
        /// Step 3 of Direct Upload: Save the video reference after successful Cloudinary upload.
        /// 
        /// After the client uploads directly to Cloudinary and receives a response,
        /// it sends the Cloudinary metadata (public_id, version, secure_url) here
        /// so we can persist the video record in our database.
        /// </summary>
        [HttpPost("save-direct-upload")]
        public async Task<IActionResult> SaveDirectUpload(
            [FromBody] SaveDirectVideoUploadRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest("Save request is required.");

                if (string.IsNullOrWhiteSpace(request.PublicId))
                    return BadRequest("Cloudinary PublicId is required.");

                if (string.IsNullOrWhiteSpace(request.SecureUrl))
                    return BadRequest("Cloudinary SecureUrl is required.");

                // Reuse the existing CreateVideoCommand to save the video in the database.
                // The VideoUrl is now the Cloudinary secure_url from the direct upload response.
                var createVideoCommand = new CreateVideoCommand()
                {
                    Name = request.Name,
                    SectionId = request.SectionId,
                    Description = request.Description,
                    VideoUrl = request.SecureUrl
                };

                var result = await _mediator.Send(createVideoCommand, cancellationToken);
                return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save direct upload video reference");
                return StatusCode(500, new { error = "An unexpected error occurred while saving the video reference." });
            }
        }

        #endregion

        #region Query Endpoints

        [HttpGet]
        public async Task<IActionResult> GetAllVideos([FromQuery] GetAllEntityRequestSkeleton request, CancellationToken cancellationToken)
        {
            var query = new GetAllVideosQuery
            {
                GetAllEntityRequestSkeleton = request
            };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        [HttpGet("{videoId:guid}")]
        public async Task<IActionResult> GetVideoById(Guid videoId, CancellationToken cancellationToken)
        {
            var query = new GetVideoByIdQuery { VideoId = videoId };

            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result.Error);
        }

        #endregion

        #region Command Endpoints

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
            var command = new DeleteVideoCommand
            {
                VideoId = videoId
            };
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpPatch("MarkVideoWatched/{VideoId}")]
        public async Task<IActionResult> MarkVideoWatched(Guid VideoId, CancellationToken cancellationToken)
        {
            var UserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!);

            var command = new VideoWatchedCommand(VideoId, UserId);

            var result = await _mediator.Send(command, cancellationToken);
            return result ? Ok() : StatusCode(500, "Failed to mark video as watched.");
        }

        #endregion
    }
}
