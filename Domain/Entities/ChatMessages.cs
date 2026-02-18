using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("chat_messages", Schema = "public")]
    public class ChatMessage : IEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("chat_room_id")]
        [ForeignKey(nameof(ChatRoom))]
        public Guid? ChatRoomId { get; set; }

        [Column("sender_id")]
        [ForeignKey(nameof(Sender))]
        public Guid? SenderId { get; set; }

        [Required]
        [Column("message_text")]
        public string MessageText { get; set; } = string.Empty;

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("star_rating")]
        public int? StarRating { get; set; }

        [Column("sent_at")]
        public DateTimeOffset SentAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public ChatRoom? ChatRoom { get; set; }
        public User? Sender { get; set; }
    }
}