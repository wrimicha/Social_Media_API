using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<string> ImagesUrls { get; set; }

        // public UserDTO(Guid id, string email, string name, List<string> imagesUrls)
        // {
        //     Id = id;
        //     Email = email;
        //     Name = name;
        //     ImagesUrls = imagesUrls;
        // }

    }
}