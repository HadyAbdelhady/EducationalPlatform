using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IChatMessageRepository : IRepository<ChatMessage>
    {
    }
}
