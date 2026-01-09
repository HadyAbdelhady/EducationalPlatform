using Application.DTOs.Sections;
using Application.Interfaces;
using Application.ResultWrapper;    
using MediatR;

namespace Application.Features.Sections.Query.GetSectionsForCourse
{
    public class GetSectionsForCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionsForCourseQuery, Result<List<SectionDetailsQueryModel>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<SectionDetailsQueryModel>>> Handle(GetSectionsForCourseQuery request, CancellationToken cancellationToken)
        {
            var sections = await _unitOfWork.GetRepository<ISectionRepository>()
                                                                    .GetSectionInnerData(request, cancellationToken);


            return Result<List<SectionDetailsQueryModel>>.Success(sections);
        }
    }
}
