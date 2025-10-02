using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public Guid? ChatRoomId { get; set; }
        public Guid? SenderId { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public int? StarRating { get; set; }
        public DateTimeOffset SentAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public ChatRoom? ChatRoom { get; set; }
        public User? Sender { get; set; }
    }
}