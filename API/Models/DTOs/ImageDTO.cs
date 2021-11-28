using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.DTOs
{
    public class ImageDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Userid { get; set; }
        public List<string> Tags { get; set; }
    }
}