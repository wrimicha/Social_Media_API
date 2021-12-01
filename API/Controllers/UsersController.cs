using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models;
using API.Models.Responses;
using API.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using API.Models.Helpers;


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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = _context.User
                            .Include(x => x.Images)
                            .ThenInclude(x => x.Tags)
                            .FirstOrDefault(x => x.Id.Equals(new Guid(id)));

            var imageCount = user.Images.Count;

            var userImages = (imageCount > 10) ? user.Images.Skip((imageCount - 10)) : user.Images;

            var imageList = new List<string>();

            foreach (var image in userImages)
            {
                imageList.Add(image.Url.ToString());
            }

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                ImagesUrls = imageList
            };

            var response = new Response<UserDTO>(userDTO);

            return Ok(response);
        }

        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetUserImages(string id, int pagenumber = 1)
        {
            var result = _context.User
                            .Include(x => x.Images)
                            .FirstOrDefault(x => x.Id.Equals(new Guid(id)));

            var pageImages = result.Images.Skip((pagenumber - 1) * 10).Take(10);

            var total = result.Images.Count();

            var userImageDTOList = new List<UserImagesDTO>();

            foreach(var image in pageImages)
            {
                userImageDTOList.Add(
                    new UserImagesDTO
                    {
                        Id = image.Id,
                        Url = image.Url,
                    }
                );
            }

            var response = ResponseHelper<UserImagesDTO>.GetPagedResponse("/api/users/" + id + "/images", userImageDTOList, pagenumber, 10, total);
            return Ok(response);
        }


        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user) //post request - must send me the RegisterDto
        {

            IsValidEmail(string email)

            await _context.User.AddAsync(new User { Email = user.Email, Name = user.Name });
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost("{id}/image")]
        public async Task<IActionResult> AddUserImage(string id, [FromBody] Image image)
        {
            var user = _context.User
                            .Include(x => x.Images)
                            .ThenInclude(x => x.Tags)
                            .SingleOrDefault(x => x.Id.Equals(new Guid(id)));

            var tags = ImageHelper.GetTags(image.Url);

            image.User = user;
            image.PostingDate = DateTime.Now;
            image.Tags = new List<Tag>();
            foreach (var tag in tags)
            {
                var existingTag = _context.Tag.FirstOrDefault(x => x.Text.ToLower() == tag);
                if (existingTag == null)
                    image.Tags.Add(new Tag { Text = tag });
                else
                    image.Tags.Add(existingTag); 
            }

            user.Images.Add(image);
            await _context.SaveChangesAsync();
            return Ok();
        }


        //TODO:update my methods to single or default

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(string id)
        {
            User user = _context.User
                            .Include(x => x.Images)
                            .SingleOrDefault(x => x.Id == new Guid(id));

            _context.RemoveRange(user.Images);

            _context.Remove(user);

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}