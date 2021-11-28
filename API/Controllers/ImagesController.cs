using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models;
using API.Models.Responses;
using Microsoft.EntityFrameworkCore;



namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {

        private readonly DataContext _context;

        public ImagesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery] int pagenumber)
        {
            var result = _context.Image
                .Skip((pagenumber - 1) * 10).Take(10);
            var total = await _context.Image.CountAsync();

            var response = ResponseHelper<Image>.GetPagedResponse("/api/images", result, pagenumber, 10, total);
            return Ok(response);
        }

    }
}