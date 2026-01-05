using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.AnswersSheets.Queries.GetAllAnswersSheetsByStudentId
{
    public class GetAllAnswersSheetsByStudentIdQueryHandler(IUnitOfWork unitOfWork) 
        : IRequestHandler<GetAllAnswersSheetsByStudentIdQuery, Result<PaginatedResult<AllAnswersSheetsByStudentResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<AllAnswersSheetsByStudentResponse>>> Handle(
            GetAllAnswersSheetsByStudentIdQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                var answersSheets = await _unitOfWork.Repository<AnswersSheet>()
                    .FindAsync(
                       x => x.StudentId == request.StudentId,
                       cancellationToken,
                       x => x.QuestionsSheet
                    );

                var filteredSheets = answersSheets.ToList();

                if (filteredSheets.Count == 0)
                {
                    return Result<PaginatedResult<AllAnswersSheetsByStudentResponse>>.FailureStatusCode(
                        $"No answers sheets found for student with ID {request.StudentId}.",
                        ErrorType.NotFound);
                }

                var response = filteredSheets.Select(a => new AllAnswersSheetsByStudentResponse
                {
                    AnswersSheetId = a.Id,
                    Name = a.Name,
                    SheetUrl = a.SheetUrl,
                    QuestionsSheetId = a.QuestionsSheetId,
                    QuestionsSheetName = a.QuestionsSheet?.Name ?? string.Empty,
                    IsApproved = a.IsApproved,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                }).ToList();

                return Result<PaginatedResult<AllAnswersSheetsByStudentResponse>>.Success(
                    new PaginatedResult<AllAnswersSheetsByStudentResponse>
                    {
                        Items = response,
                        PageNumber = 1,
                        PageSize = response.Count,
                        TotalCount = response.Count
                    });
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<AllAnswersSheetsByStudentResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving answers sheets: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}

