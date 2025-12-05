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

namespace Application.Features.Videos.Commands.DeleteVideo
{
    public class DeleteVideoCommandHandler : IRequestHandler<DeleteVideoCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteVideoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<string>> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
        {
           await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var video = await _unitOfWork.Repository<Video>().GetByIdAsync(request.VideoId,cancellationToken) ?? throw new KeyNotFoundException("Video not found");

               await _unitOfWork.Repository<Video>().RemoveAsync(request.VideoId, cancellationToken);
                var Result = await _unitOfWork.SaveChangesAsync(cancellationToken);
               await _unitOfWork.CommitTransactionAsync(cancellationToken);

                if (Result > 0)
                    return Result<string>.Success("Video Deleted Successfully");

                return Result<string>.FailureStatusCode("Failed To Delete Video", ErrorType.BadRequest);

            }
            catch(KeyNotFoundException ex)
            {
                return Result<string>.FailureStatusCode(ex.Message, ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<string>.FailureStatusCode($"An error occurred while deleting the video: {ex.Message}", ErrorType.InternalServerError);

            }
        }
    }
}
