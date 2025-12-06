using Domain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Review
{
    public class ReviewDeletionRequest
    {
        public Guid ReviewId { get; set; }
        public ReviewEntityType EntityType { get; set; }
    }
}
