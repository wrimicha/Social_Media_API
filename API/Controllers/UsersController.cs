using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(_context.User);
        }


        [HttpPost]
        public async Task<IActionResult> AddUser([FromQuery] string name, string email) //post request - must send me the RegisterDto
        {
            //var user = new User(email, name);
            var user = new User
            {
                Email = email,
                Name = name,
            };

            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}