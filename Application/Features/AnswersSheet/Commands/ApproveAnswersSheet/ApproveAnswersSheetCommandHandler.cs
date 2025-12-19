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

namespace Application.Features.AnswersSheets.Commands.ApproveAnswersSheet
{
    public class ApproveAnswersSheetCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ApproveAnswersSheetCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(ApproveAnswersSheetCommand request, CancellationToken cancellationToken)
        {
            var answersSheet = await _unitOfWork.Repository<AnswersSheet>().GetByIdAsync(request.AnswersSheetId, cancellationToken);

            if (answersSheet is null)
            {
                return Result<string>.FailureStatusCode("Answers sheet not found", ErrorType.NotFound);
            }

            var questionsSheet = await _unitOfWork.Repository<Sheet>().GetByIdAsync(answersSheet.QuestionsSheetId, cancellationToken);

            if (request.InstructorId != questionsSheet?.InstructorId)
            {
                return Result<string>.FailureStatusCode("You're unauthorized to approve this answers sheet", ErrorType.UnAuthorized);
            }

            answersSheet.IsApproved = true;

            _unitOfWork.Repository<AnswersSheet>().Update(answersSheet);
            await _unitOfWork.Repository<AnswersSheet>().SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Answers sheet approved successfully.");
        }
    }
}
