using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModelTrial.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [ForeignKey("FK_User")]
        public int UserId { get; set; }
        [ForeignKey("FK_Event")]
        public int EventId { get; set; }
    }
}
