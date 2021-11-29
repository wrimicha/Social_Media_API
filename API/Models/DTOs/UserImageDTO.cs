using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.DTOs
{
    public class UserImagesDTO
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        
    }
}