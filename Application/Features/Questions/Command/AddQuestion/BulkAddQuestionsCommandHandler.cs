using Application.Common;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Command.AddQuestion
{
    public class BulkAddQuestionsHandler(IUnitOfWork unitOfWork, ICloudinaryCore cloudinaryService)
        : IRequestHandler<BulkAddQuestionsCommand, Result<List<Guid>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<List<Guid>>> Handle(BulkAddQuestionsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var questions = new List<Question>();

                foreach (var item in request.Questions)
                {
                    string? pictureUrl = item.PictureUrl;

                    if (item.PictureFile is not null)
                        pictureUrl = await _cloudinaryService.UploadMediaAsync(item.PictureFile, UsageCategory.CourseThumbnail);

                    var question = new Question
                    {
                        Id = Guid.NewGuid(),
                        QuestionString = item.QuestionString,
                        QuestionImageUrl = pictureUrl,
                        SectionId = request.SectionId,
                        CourseId = request.CourseId,
                    };

                    for (int i = 0; i < item.AnswerTexts.Count; i++)
                    {
                        question.Answers.Add(new Answer
                        {
                            Id = Guid.NewGuid(),
                            QuestionId = question.Id,
                            AnswerText = item.AnswerTexts[i],
                            IsCorrect = item.IsCorrects[i],
                            Explanation = item.Explanation
                        });
                    }

                    questions.Add(question);
                }

                await _unitOfWork.Repository<Question>().AddRangeAsync(questions, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<List<Guid>>.Success(questions.Select(q => q.Id).ToList());
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<List<Guid>>.FailureStatusCode(ex.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<List<Guid>>.FailureStatusCode(
                    $"An error occurred while bulk creating questions: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}

