using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Questions.Command.AddQuestion
{
    public class AddQuestionHandler(IUnitOfWork context, ICloudinaryCore cloudinaryService) : IRequestHandler<AddQuestionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork = context;
        private readonly ICloudinaryCore _cloudinaryService = cloudinaryService;

        public async Task<Result<Guid>> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string? pictureUrl = request.PictureUrl;

                if (request.PictureFile is not null)
                {
                    pictureUrl = await _cloudinaryService.UploadMediaAsync(request.PictureFile, UsageCategory.CourseThumbnail);
                }
                // 1. Create the Question Entity
                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    QuestionString = request.QuestionString,
                    QuestionImageUrl = pictureUrl,
                    SectionId = request.SectionId,
                    CourseId = request.CourseId,
                };

                // 2. Create the Answer Entities
                for (int i = 0; i < request.AnswerTexts.Count; i++)
                {
                    var answer = new Answer
                    {
                        Id = Guid.NewGuid(),
                        QuestionId = question.Id, // Link to parent
                        AnswerText = request.AnswerTexts[i],
                        IsCorrect = request.IsCorrects[i],
                        Explanation = request.Explanation
                    };
                    question.Answers.Add(answer);
                }
                await _unitOfWork.Repository<Question>().AddAsync(question, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(question.Id);
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<Guid>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<Guid>.FailureStatusCode($"Error creating Question: {ex.Message}", ErrorType.Conflict);
            }
        }
    }
}
