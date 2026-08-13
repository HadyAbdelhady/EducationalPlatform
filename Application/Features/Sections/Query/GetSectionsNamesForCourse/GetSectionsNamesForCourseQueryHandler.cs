using Application.Common.Interfaces;
using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Sections.Query.GetSectionsNamesFourCourse
{
    public class GetSectionsNamesForCourseQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetSectionsNamesForCourseQuery, Result<List<SectionData>>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<List<SectionData>>> Handle(GetSectionsNamesForCourseQuery request, CancellationToken cancellationToken)
        {
            var sections = await _unitOfWork.Repository<Section>().GetAll(cancellationToken)
                                                           .Where(s => s.CourseId == request.CourseId)
                                                           .Select(s => new SectionData { Id = s.Id, Name = s.Name })
                                                           .ToListAsync(cancellationToken);

            return Result<List<SectionData>>.Success(sections);
        }
    }
}
