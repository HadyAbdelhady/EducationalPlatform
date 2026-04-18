using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Command.UpdateQuestion
{
    public class UpdateQuestionHandler(
        IUnitOfWork unitOfWork,
        IQuestionUpdateService questionUpdateService,
        ICloudinaryCore cloudinaryService) : IRequestHandler<UpdateQuestionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IQuestionUpdateService _questionUpdateService = questionUpdateService;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<Guid>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingQuestion = await _unitOfWork.Repository<Question>()
                    .GetByIdAsync(request.QuestionId, cancellationToken, q => q.Answers);

                if (existingQuestion == null)
                {
                    return Result<Guid>.FailureStatusCode("Question not found.", ErrorType.NotFound);
                }

                string? finalImageUrl;
                if (request.PictureFile is not null)
                {
                    finalImageUrl = await _cloudinaryService.UploadMediaAsync(request.PictureFile, UsageCategory.CourseThumbnail);
                }
                else if (request.QuestionImageUrl is not null)
                {
                    finalImageUrl = request.QuestionImageUrl;
                }
                else
                {
                    finalImageUrl = existingQuestion.QuestionImageUrl;
                }

                _questionUpdateService.UpdateQuestion(
                    existingQuestion,
                    request.QuestionString,
                    finalImageUrl,
                    request.Answers);

                _unitOfWork.Repository<Question>().Update(existingQuestion);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(existingQuestion.Id);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<Guid>.FailureStatusCode(ex.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<Guid>.FailureStatusCode(
                    $"Failed to update question: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}