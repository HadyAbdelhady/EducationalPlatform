using Application.ResultWrapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AnswersSheet.Commands.ApproveAnswersSheet
{
    public class ApproveAnswersSheetCommand :  IRequest<Result<string>>
    {
        public Guid AnswersSheetId { get; set; }
        public Guid InstructorId { get; set; }
    }
}
