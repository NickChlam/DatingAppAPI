using System.ComponentModel.DataAnnotations;
using System;

namespace DatingApp.API.Dtos
{
    public class UserForRegister
    {
        [Required]
        
        public string UserName { get; set; }
        
        [Required]
        [StringLength(9, MinimumLength = 4, ErrorMessage = "You must set a password between 4-8 charachters")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserForRegister()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;

        }

        

    }
}