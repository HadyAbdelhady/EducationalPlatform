using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Section;
using Application.ResultWrapper;
using MediatR;

namespace Application.Features.Section.Query.GetSectionsForCourse
{
    public record GetSectionsForCourseQuery(Guid CourseId): IRequest<Result<List<GetSectionResponse>>>;
}
