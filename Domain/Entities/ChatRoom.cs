using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;

namespace Domain.Entities
{
    [Table("chat_rooms", Schema = "public")]
    public class ChatRoom : ISoftDeletableEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user1_id")]
        [ForeignKey(nameof(User1))]
        public Guid? User1Id { get; set; }

        [Column("user2_id")]
        [ForeignKey(nameof(User2))]
        public Guid? User2Id { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        public User? User1 { get; set; }
        public User? User2 { get; set; }
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}