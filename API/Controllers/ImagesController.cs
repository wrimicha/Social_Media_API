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
using System.Text.RegularExpressions;


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
        public async Task<IActionResult> GetPage([FromQuery] int pagenumber = 1, string tag = null)
        {
            var result = _context.Image
                .OrderBy(x => x.PostingDate)
                .Include(x => x.User)
                .Skip((pagenumber - 1) * 10).Take(10);

            var total = await result.CountAsync();

            var imageDTOList = new List<ImagesDTO>();

            foreach (var image in result)
            {
                imageDTOList.Add(
                    new ImagesDTO
                    {
                        Id = image.Id,
                        Url = image.Url,
                        Username = image.User.Name.ToString()
                    }
                );
            }

            var response = ResponseHelper<ImagesDTO>.GetPagedResponse("/api/images", imageDTOList, pagenumber, 10, total);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPage(string id, [FromQuery] int pagenumber = 1)
        {

            Regex rgx = new Regex(@"^[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12}$");

            if (!rgx.IsMatch(id))
            {
                return BadRequest(new ErrorDTO
                {
                    Status = "400",
                    Title = "The id format is not correct.",
                    Detail = "Please enter an id following the XXXXXXXX-XXXX-XXXX-XXXXXXXXXXXX format"
                });
            }
            var result = _context.Image
                        .OrderBy(x => x.PostingDate)
                        .Include(x => x.Tags)
                        .Include(x => x.User)
                        .FirstOrDefault(x => x.Id.Equals(new Guid(id)));

            if (result == null)
            {
                return NotFound(new ErrorDTO
                {
                    Status = "404",
                    Title = "Id does not exist",
                    Detail = "Ensure that a valid id which corresponds to an existing user in the system is provided"
                });
            }

            var tagList = new List<string>();

            foreach (var tag in result.Tags)
            {
                tagList.Add(tag.Text.ToString());
            }

            var imageDTO = new ImageDTO
            {
                Id = result.Id,
                Url = result.Url,
                Username = result.User.Name.ToString(),
                Userid = result.User.Id.ToString(),
                Tags = tagList
            };
            return Ok(imageDTO);
        }

        [HttpGet("byTag")]
        public async Task<IActionResult> GetByTag([FromQuery] int pagenumber = 1, string tag = null)
        {
            var result = _context.Image
                .OrderBy(x => x.PostingDate)
                .Include(x => x.User)
                .Include(x => x.Tags)
                .Where(x => x.Tags.Any(y => y.Text.ToLower().Equals(tag.ToLower())))
                .Skip((pagenumber - 1) * 10).Take(10);

            if (result.Count() == 0)
            {
                return NotFound(new ErrorDTO
                {
                    Status = "404",
                    Title = "No tags found.",
                    Detail = "There are currently no images with the tag " + tag
                });
            }

            var total = await result.CountAsync();

            var imageDTOList = new List<ImagesDTO>();

            foreach (var image in result)
            {
                imageDTOList.Add(
                    new ImagesDTO
                    {
                        Id = image.Id,
                        Url = image.Url,
                        Username = image.User.Name.ToString()
                    }
                );
            }

            var response = ResponseHelper<ImagesDTO>.GetPagedResponse("/api/images", imageDTOList, pagenumber, 10, total);
            return Ok(response);
        }


        [HttpGet("populartags")]
        public async Task<IActionResult> PopularTags()
        {
            var result = _context.Tag
                            .Include(x => x.Images)
                            .OrderByDescending(x => x.Images.Count)
                            .ThenBy(x => x.Text)
                            .Take(5);

            var poptags = new List<TagCountDTO>();

            foreach (var tag in result)
            {
                poptags.Add(
                     new TagCountDTO
                     {
                         Tag = tag.Text,
                         Count = tag.Images.Count.ToString()
                     }
                );
            }

            return Ok(poptags);
        }
    }
}