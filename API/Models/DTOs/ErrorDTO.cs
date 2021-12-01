using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.DTOs
{
    public class ErrorDTO
    {
        public string Status { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }
}