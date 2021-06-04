using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ModelTrial.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public User CreatedByUser { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
