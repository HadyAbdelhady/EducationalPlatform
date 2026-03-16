using Application.Interfaces;
using Application.ResultWrapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Sections.Query.GetSectionsNamesFourCourse
{
    public class GetSectionsNamesFourCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionsNamesFourCourseQuery, Result<List<SectionData>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<SectionData>>> Handle(GetSectionsNamesFourCourseQuery request, CancellationToken cancellationToken)
        {
            var sections = _unitOfWork.Repository<Section>().GetAll(cancellationToken)
                                                           .Where(s => s.CourseId == request.CourseId)
                                                           .Select(s => new SectionData { Id = s.Id, Name = s.Name })
                                                           .ToList();

            return Result<List<SectionData>>.Success(sections);
        }
    }
}
