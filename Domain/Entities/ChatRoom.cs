using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class ChatRoom
    {
        public Guid Id { get; set; }
        public Guid? User1Id { get; set; }
        public Guid? User2Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public User? User1 { get; set; }
        public User? User2 { get; set; }
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}