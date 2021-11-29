using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTOs
{
    public class TagDTO
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<string> Imageurls { get; set; }
    }
}