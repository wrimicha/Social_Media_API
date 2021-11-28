using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models;
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

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(_context.User);
        }


        //TODO: set limit of returned images to 10
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = _context.User
                            //.FindAsync(new Guid(id));
                            .Include(x => x.Images)
                            .ThenInclude(x => x.Tags)
                            .FirstOrDefault(x => x.Id.Equals(new Guid(id)));
            return Ok(user);
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


        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = _context.User.FirstOrDefault(x => x.Id.Equals(new Guid(id)));
                 
                 
            var images = user.Images;
                        
            return Ok(images);
        }


        [HttpPost("{id}/image")]
        public async Task<IActionResult> AddUserImage(string id, [FromBody] Image image)
        {

            //get the image from the body or generate it yourself maybe?

            //var image = "https://picsum.photos/200/300";


            //use the ImageHelper.GetTags function


            //Get the specific user for which you need to add the image
            //var user = _context.User.Find(new Guid(id));


            var user = _context.User
                            .Include(x => x.Images)
                            .ThenInclude(x => x.Tags)
                            .SingleOrDefault(x => x.Id.Equals(new Guid(id)));


            var tags = ImageHelper.GetTags(image.Url);
        
            //var tagsList = new List<Tag>();

            // var imageObject = new Image
            // {
                
            // }; 

            image.Url = image.Url;
            image.User = user;
            image.PostingDate = DateTime.Now;

            //Image.Tag = List<Tag>();

            foreach(var tag in tags)
            {
                var existingTag = _context.Tag.FirstOrDefault(x => x.Text.ToLower() == tag);
                if (existingTag == null){
                    // tagsList.Add(newTag);
                    var newTag = new Tag{Text = tag};
                    // await _context.Tag.AddAsync(newTag);
                    image.Tags.Add(newTag);
                    newTag.Images.Add(image);
                }
                else{
                    //tagsList.Add(existingTag);
                    image.Tags.Add(existingTag);
                    existingTag.Images.Add(image);
                    //existingTag.Image = imageObject;
                }
            }

            // var imageObject = new Image
            // {
            //     Url = image.Url,
            //     User = user,
            //     PostingDate = DateTime.Now,
            //     Tags = tagsList
            // };            

            // //add the image to the user
            user.Images.Add(image);

            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}