using Application.DTOs.Sheets;
using Application.Features.AnswersSheets.Commands.ApproveAnswersSheet;
using Application.Features.AnswersSheets.Commands.CreateAnswersSheet;
using Application.Features.AnswersSheets.Commands.DeleteAnswersSheet;
using Application.Features.AnswersSheets.Commands.UpdateAnswersSheet;
using Application.Features.Sheets.Commands.CreateSheet;
using Application.Features.Sheets.Commands.DeleteSheet;
using Application.Features.Sheets.Commands.UpdateSheet;
using Application.Features.Sheets.Queries.GetAllTutorialSheetsByCourse;
using Application.Features.Sheets.Queries.GetAllTutorialSheetsByVideo;
using Application.Features.Sheets.Queries.GetAllTutorialSheetsBySection;
using Application.Features.Sheets.Queries.GetAllQuestionSheetsByCourse;
using Application.Features.Sheets.Queries.GetAllQuestionSheetsByVideo;
using Application.Features.Sheets.Queries.GetAllQuestionSheetsBySection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Features.AnswersSheets.Queries.GetAllAnswersSheetsByStudentId;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SheetsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        private bool TryGetCurrentUserId(out Guid userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out userId))
            {
                userId = default;
                return false;
            }

            return true;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSheet(SheetCreationRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return BadRequest("Sheet creation request can not be null.");
            }
            if(request.InstructorId == Guid.Empty)
            {
                return BadRequest("Instructor Id can not be null.");
            }
            if(request.Type == Domain.enums.SheetType.QuestionSheet && (request.DueDate is null || request.DueDate < DateTimeOffset.UtcNow))
            {
                return BadRequest("Due Date can not be null or in the past.");
            }
            if (request.CourseId is null && request.SectionId is null && request.VideoId is null)
            {
                return BadRequest("Sheet must be associated with a course, section, or video. Please provide at least one valid identifier.");
            }

            var providedTargetsCount = new[]
            {
                request.CourseId,
                request.SectionId,
                request .VideoId
            }.Count(id => id is not null);

            if (providedTargetsCount != 1)
            {
                return BadRequest(
                    "Sheet must be associated with exactly one target: Course, Section, or Video."
                );
            }

            var SheetCommand = new CreateSheetCommand
            {
                Name = request.Name,
                SheetUrl = request.SheetFile,
                Type = request.Type,
                DueDate = request.DueDate,
                InstructorId = request.InstructorId,
                SectionId = request.SectionId,
                VideoId = request.VideoId,
                CourseId = request.CourseId
            };

            var result = await _mediator.Send(SheetCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateSheet(SheetUpdateRequest sheetUpdateRequest, CancellationToken cancellationToken)
        {
            if (sheetUpdateRequest == null)
            {
                return BadRequest("Sheet update request can not be null.");
            }
     
            var SheetCommand = new UpdateSheetCommand
            {
               SheetId = sheetUpdateRequest.SheetId,
               Name= sheetUpdateRequest.Name,
               SheetUrl= sheetUpdateRequest.SheetFile
            };

            var result = await _mediator.Send(SheetCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSheet(Guid sheetId, CancellationToken cancellationToken)
        {
            var sheetCommand = new DeleteSheetCommand
            {
                SheetId = sheetId
            };

            var result = await _mediator.Send(sheetCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        // AnswersSheet Endpoints
        [HttpPost("answers")]
        public async Task<IActionResult> CreateAnswersSheet(AnswersSheetCreationRequest answersSheetCreationRequest, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized();
            }

            if (answersSheetCreationRequest == null)
            {
                return BadRequest("Answers sheet creation request can not be null.");
            }

            if (answersSheetCreationRequest.QuestionsSheetId == Guid.Empty)
            {
                return BadRequest("Questions sheet ID is required.");
            }

            if (answersSheetCreationRequest.SheetFile is null || answersSheetCreationRequest.SheetFile.Length == 0)
            {
                return BadRequest("A PDF file is required.");
            }

            var answersSheetCommand = new CreateAnswersSheetCommand
            {
                Name = answersSheetCreationRequest.Name,
                SheetFile = answersSheetCreationRequest.SheetFile,
                QuestionsSheetId = answersSheetCreationRequest.QuestionsSheetId,
                StudentId = userId
            };

            var result = await _mediator.Send(answersSheetCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPatch("answers")]
        public async Task<IActionResult> UpdateAnswersSheet(AnswersSheetUpdateRequest answersSheetUpdateRequest, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized();
            }

            if (answersSheetUpdateRequest == null)
            {
                return BadRequest("Answers sheet update request can not be null.");
            }

            if (answersSheetUpdateRequest.AnswersSheetId == Guid.Empty)
            {
                return BadRequest("Answers sheet ID is required.");
            }

            if (answersSheetUpdateRequest.SheetFile is null || answersSheetUpdateRequest.SheetFile.Length == 0)
            {
                return BadRequest("A PDF file is required.");
            }

            var answersSheetCommand = new UpdateAnswersSheetCommand
            {
                AnswersSheetId = answersSheetUpdateRequest.AnswersSheetId,
                StudentId = userId,
                Name = answersSheetUpdateRequest.Name,
                SheetFile = answersSheetUpdateRequest.SheetFile
            };

            var result = await _mediator.Send(answersSheetCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("answers/{answersSheetId}")]
        public async Task<IActionResult> DeleteAnswersSheet(Guid answersSheetId, CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized();
            }

            var answersSheetCommand = new DeleteAnswersSheetCommand
            {
                AnswersSheetId = answersSheetId,
                StudentId = userId
            };

            var result = await _mediator.Send(answersSheetCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpGet("answers/Approve/{answersSheetId}")]
        public async Task<IActionResult> ApproveAnswersSheet(Guid answersSheetId, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var answersSheetCommand = new ApproveAnswersSheetCommand
            {
                AnswersSheetId = answersSheetId,
                InstructorId = Guid.Parse(userIdClaim!)
            };

            var result = await _mediator.Send(answersSheetCommand, cancellationToken);
            return result.IsSuccess? Ok(result) : StatusCode((int)result.ErrorType, result);
        }


        // QUERIES
        [HttpGet("GetAllTutorialSheetsByCourse/{courseId}")]
        public async Task<IActionResult> GetAllTutorialSheetsByCourse(Guid courseId, CancellationToken cancellationToken)
        {
            if(courseId == Guid.Empty)
                return BadRequest("Course Id can not be empty");

            var query = new GetAllTutorialSheetsByCourseQuery { CourseId = courseId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetAllTutorialSheetsByVideo/{videoId}")]
        public async Task<IActionResult> GetAllTutorialSheetsByVideo(Guid videoId, CancellationToken cancellationToken)
        {
            if(videoId == Guid.Empty)
                return BadRequest("Video Id can not be empty");

            var query = new GetAllTutorialSheetsByVideoQuery { VideoId = videoId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetAllTutorialSheetsBySection/{sectionId}")]
        public async Task<IActionResult> GetAllTutorialSheetsBySection(Guid sectionId, CancellationToken cancellationToken)
        {
            if(sectionId == Guid.Empty)
                return BadRequest("Section Id can not be empty");

            var query = new GetAllTutorialSheetsBySectionQuery { SectionId = sectionId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetAllQuestionSheetsByCourse/{courseId}")]
        public async Task<IActionResult> GetAllQuestionSheetsByCourse(Guid courseId, CancellationToken cancellationToken)
        {
            if(courseId == Guid.Empty)
                return BadRequest("Course Id can not be empty");

            var query = new GetAllQuestionSheetsByCourseQuery { CourseId = courseId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetAllQuestionSheetsByVideo/{videoId}")]
        public async Task<IActionResult> GetAllQuestionSheetsByVideo(Guid videoId, CancellationToken cancellationToken)
        {
            if(videoId == Guid.Empty)
                return BadRequest("Video Id can not be empty");

            var query = new GetAllQuestionSheetsByVideoQuery { VideoId = videoId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetAllQuestionSheetsBySection/{sectionId}")]
        public async Task<IActionResult> GetAllQuestionSheetsBySection(Guid sectionId, CancellationToken cancellationToken)
        {
            if(sectionId == Guid.Empty)
                return BadRequest("Section Id can not be empty");

            var query = new GetAllQuestionSheetsBySectionQuery { SectionId = sectionId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

        [HttpGet("GetAllAnswerSheetsByStudent")]
        public async Task<IActionResult> GetAllAnswersSheetsByStudent( CancellationToken cancellationToken)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized();
            }

            var query = new GetAllAnswersSheetsByStudentIdQuery { StudentId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return result.IsSuccess ? Ok(result) : NotFound(result.Error);
        }

    }
}
