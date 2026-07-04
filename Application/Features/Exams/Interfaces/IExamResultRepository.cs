using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Exams.Interfaces
{
    public interface IExamResultRepository : IRepository<StudentExamResult>
    {
    }
}
