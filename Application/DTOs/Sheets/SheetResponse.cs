using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Sheets
{
    public class SheetResponse : SheetItem
    {
        public DateTimeOffset? DueDate { get; set; }
    }
}
