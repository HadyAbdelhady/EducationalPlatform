using Application.DTOs.Sheets;
using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Sheets.Queries.GetAllTutorialSheetsByVideo
{
    public class GetAllTutorialSheetsByVideoQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllTutorialSheetsByVideoQuery, Result<PaginatedResult<SheetResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<PaginatedResult<SheetResponse>>> Handle(GetAllTutorialSheetsByVideoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var videoExists = await _unitOfWork.Repository<Video>().AnyAsync(x => x.Id == request.VideoId, cancellationToken);
                if (!videoExists)
                    return Result<PaginatedResult<SheetResponse>>.FailureStatusCode("Video not found", ErrorType.NotFound);

                var tutorialSheets = await _unitOfWork.GetRepository<ISheetRepository>().GetAllSheetsByVideoAsync(request.VideoId, SheetType.TutorialSheet, cancellationToken);

                return Result<PaginatedResult<SheetResponse>>.Success(new PaginatedResult<SheetResponse>
                {
                    Items = tutorialSheets.ToList(),
                    PageNumber = 1,
                    PageSize = tutorialSheets.Count,
                    TotalCount = tutorialSheets.Count
                });
            }
            catch (UnauthorizedAccessException auth)
            {
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(auth.Message, ErrorType.UnAuthorized);
            }
            catch (Exception ex)
            {
                return Result<PaginatedResult<SheetResponse>>.FailureStatusCode(
                    $"An error occurred while retrieving tutorial sheets: {ex.Message}",
                    ErrorType.InternalServerError);
            }
        }
    }
}



