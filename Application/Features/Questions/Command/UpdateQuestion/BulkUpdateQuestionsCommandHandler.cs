using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Questions.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Command.UpdateQuestion
{
    public class BulkUpdateQuestionsHandler(
        IUnitOfWork unitOfWork,
        IQuestionUpdateService questionUpdateService,
        ICloudinaryCore cloudinaryService)
        : IRequestHandler<BulkUpdateQuestionsCommand, Result<List<Guid>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IQuestionUpdateService _questionUpdateService = questionUpdateService;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<List<Guid>>> Handle(BulkUpdateQuestionsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var updatedQuestions = new List<Question>();

                foreach (var item in request.Questions)
                {
                    var existing = await _unitOfWork.Repository<Question>()
                        .GetByIdAsync(item.QuestionId, cancellationToken, q => q.Answers);

                    if (existing is null)
                        return Result<List<Guid>>.FailureStatusCode(
                            $"Question {item.QuestionId} not found.",
                            ErrorType.NotFound);

                    string? finalImageUrl;
                    if (item.PictureFile is not null)
                        finalImageUrl = await _cloudinaryService.UploadMediaAsync(item.PictureFile, UsageCategory.CourseThumbnail);
                    else if (item.QuestionImageUrl is not null)
                        finalImageUrl = item.QuestionImageUrl;
                    else
                        finalImageUrl = existing.QuestionImageUrl;

                    _questionUpdateService.UpdateQuestion(existing, item.QuestionString, finalImageUrl, item.Answers);
                    updatedQuestions.Add(existing);
                }

                _unitOfWork.Repository<Question>().UpdateRange(updatedQuestions);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<List<Guid>>.Success(updatedQuestions.Select(q => q.Id).ToList());
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<List<Guid>>.FailureStatusCode(ex.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<List<Guid>>.FailureStatusCode(
                    $"Failed to bulk update questions: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}

