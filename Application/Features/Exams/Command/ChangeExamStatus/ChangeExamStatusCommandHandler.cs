using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Exams.Command.ChangeExamStatus
{
    public class ChangeExamStatusCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeExamStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<bool> Handle(ChangeExamStatusCommand request, CancellationToken cancellationToken)
        {
            var examEntity = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.ExamId, cancellationToken);
            if (examEntity == null)
                return false;

            examEntity.Status = request.Status;
            _unitOfWork.Repository<Exam>().Update(examEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
