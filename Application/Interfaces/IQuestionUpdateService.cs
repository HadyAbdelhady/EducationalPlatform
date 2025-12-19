using Application.DTOs.Answer;
using Domain.Entities;

namespace Application.Interfaces
{
    /// <summary>
    /// Service for updating question entities and their associated answers.
    /// This service handles the business logic for updating question properties
    /// and managing answer collections (update, add, soft-delete).
    /// </summary>
    public interface IQuestionUpdateService
    {
        /// <summary>
        /// Updates a question entity with new properties and manages its answers.
        /// </summary>
        /// <param name="question">The question entity to update.</param>
        /// <param name="questionString">The new question text.</param>
        /// <param name="questionImageUrl">The new question image URL (optional).</param>
        /// <param name="answers">The list of answers to update/add/remove.</param>
        void UpdateQuestion(Question question, string questionString, string? questionImageUrl, List<UpdateAnswerDto> answers);
    }
}

