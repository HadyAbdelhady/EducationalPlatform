using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using Domain.enums;
using MediatR;

namespace Application.Features.Videos.Commands.DeleteVideo
{
    public class DeleteVideoCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteVideoCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<string>> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var video = await _unitOfWork.Repository<Video>().GetByIdAsync(request.VideoId, cancellationToken) ?? throw new KeyNotFoundException("Video not found");

                await _unitOfWork.Repository<Video>().RemoveAsync(request.VideoId, cancellationToken);
                var Result = await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                if (Result > 0)
                    return Result<string>.Success("Video Deleted Successfully");

                return Result<string>.FailureStatusCode("Failed To Delete Video", ErrorType.BadRequest);

            }
            catch (KeyNotFoundException ex)
            {
                return Result<string>.FailureStatusCode(ex.Message, ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                return Result<string>.FailureStatusCode($"An error occurred while deleting the video: {ex.Message}", ErrorType.InternalServerError);

            }
        }
    }
}
