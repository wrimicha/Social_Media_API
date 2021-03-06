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
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

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


        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            //verify email address
            var email_format = new EmailAddressAttribute();
            if(!email_format.IsValid(user.Email))
            {
                return BadRequest(new ErrorDTO
                {
                    Status = "400",
                    Title = "Email address is not a valid format",
                    Detail = "Please enter an email address which is in t"
                });
            }
            if(_context.User.SingleOrDefault(x => x.Email.ToLower().Equals(user.Email)) != null)
            {
                return BadRequest(new ErrorDTO
                {
                    Status = "400",
                    Title = "Email address already exists",
                    Detail = "Please enter a new email address"
                });
            }

            //add user
            await _context.User.AddAsync(new User { Email = user.Email, Name = user.Name });
            await _context.SaveChangesAsync();
            return Ok(); 
        }


        [HttpPost("{id}/image")]
        public async Task<IActionResult> AddUserImage(string id, [FromBody] Image image)
        {

            //get the user by id
            var user = _context.User
                            .Include(x => x.Images)
                            .ThenInclude(x => x.Tags)
                            .SingleOrDefault(x => x.Id.Equals(new Guid(id)));

            //get tags for the image
            var tags = ImageHelper.GetTags(image.Url);

            //build the image object from the user
            image.User = user;
            image.PostingDate = DateTime.Now;
            image.Tags = new List<Tag>();

            //assign tags to list of tags
            foreach (var tag in tags)
            {
                var existingTag = _context.Tag.FirstOrDefault(x => x.Text.ToLower() == tag);
                if (existingTag == null)
                    image.Tags.Add(new Tag { Text = tag });
                else
                    image.Tags.Add(existingTag); 
            }
            
            //add image to user
            user.Images.Add(image);

            var imageCount = user.Images.Count;

            var userImages = (imageCount > 10) ? user.Images.Skip((imageCount - 10)) : user.Images;

            var imageList = new List<string>();
            
            //add images to imageList of strings
            foreach (var img in userImages)
            {
                imageList.Add(img.Url.ToString());
            }

            //create the user DTO object
            var userDTO = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Name,
                ImagesUrls = imageList
            };

            await _context.SaveChangesAsync();
            return Ok(userDTO);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            //Get user by id
            var user = _context.User
                            .Include(x => x.Images)
                            .ThenInclude(x => x.Tags)
                            .SingleOrDefault(x => x.Id.Equals(new Guid(id)));

            var imageCount = user.Images.Count;

            //take 10 images or all if less than 10
            var userImages = (imageCount > 10) ? user.Images.Skip((imageCount - 10)) : user.Images;


            var imageList = new List<string>();

            //add images to image list
            foreach (var image in userImages)
            {
                imageList.Add(image.Url.ToString());
            }

            var userDTO = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Name,
                ImagesUrls = imageList
            };

            //var response = new Response<UserDTO>();

            return Ok(userDTO);
        }

        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetUserImages(string id, int pagenumber = 1)
        {
            
            //check for the user id format
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

            var result = _context.User
                            .Include(x => x.Images.OrderBy(y => y.PostingDate))
                            .SingleOrDefault(x => x.Id.Equals(new Guid(id)));

            if (result == null)
            {
                return NotFound(new ErrorDTO
                {
                    Status = "404",
                    Title = "No user found.",
                    Detail = "The user id you entered did not match any in our database"
                });
                
            }

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