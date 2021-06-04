using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModelTrial.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}
