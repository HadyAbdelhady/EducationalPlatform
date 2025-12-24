using Application.DTOs.Sheets;
using Application.Features.AnswersSheets.Commands.ApproveAnswersSheet;
using Application.Features.AnswersSheets.Commands.CreateAnswersSheet;
using Application.Features.AnswersSheets.Commands.DeleteAnswersSheet;
using Application.Features.AnswersSheets.Commands.UpdateAnswersSheet;
using Application.Features.Sheets.Commands.CreateSheet;
using Application.Features.Sheets.Commands.DeleteSheet;
using Application.Features.Sheets.Commands.UpdateSheet;
using Application.ResultWrapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Edu_Base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SheetsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

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
            if(request.Type == Domain.enums.SheetType.QuestionSheet && (request.DueDate is null || request.DueDate < DateTime.Now))
            {
                return BadRequest("Due Date can not be null.");
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
            if (answersSheetCreationRequest == null)
            {
                return BadRequest("Answers sheet creation request can not be null.");
            }

            if (answersSheetCreationRequest.QuestionsSheetId == Guid.Empty)
            {
                return BadRequest("Questions sheet ID is required.");
            }

            if (answersSheetCreationRequest.StudentId == Guid.Empty)
            {
                return BadRequest("Student ID is required.");
            }

            var answersSheetCommand = new CreateAnswersSheetCommand
            {
                Name = answersSheetCreationRequest.Name,
                SheetFile = answersSheetCreationRequest.SheetFile,
                QuestionsSheetId = answersSheetCreationRequest.QuestionsSheetId,
                StudentId = answersSheetCreationRequest.StudentId
            };

            var result = await _mediator.Send(answersSheetCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpPatch("answers")]
        public async Task<IActionResult> UpdateAnswersSheet(AnswersSheetUpdateRequest answersSheetUpdateRequest, CancellationToken cancellationToken)
        {
            if (answersSheetUpdateRequest == null)
            {
                return BadRequest("Answers sheet update request can not be null.");
            }

            var answersSheetCommand = new UpdateAnswersSheetCommand
            {
                AnswersSheetId = answersSheetUpdateRequest.AnswersSheetId,
                Name = answersSheetUpdateRequest.Name,
                SheetFile = answersSheetUpdateRequest.SheetFile
            };

            var result = await _mediator.Send(answersSheetCommand, cancellationToken);
            return result.IsSuccess ? Ok(result) : StatusCode((int)result.ErrorType, result);
        }

        [HttpDelete("answers/{answersSheetId}")]
        public async Task<IActionResult> DeleteAnswersSheet(Guid answersSheetId, CancellationToken cancellationToken)
        {
            var answersSheetCommand = new DeleteAnswersSheetCommand
            {
                AnswersSheetId = answersSheetId
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
    }
}
