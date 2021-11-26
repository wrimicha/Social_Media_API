using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.DTOs
{
    public class UserDto
    {

        public string Email { get; set; }
        public string Name { get; set; }

        //if every thing is good we will send this back (to the client?) (JWT)
        //public string Token { get; set; }
    }
}