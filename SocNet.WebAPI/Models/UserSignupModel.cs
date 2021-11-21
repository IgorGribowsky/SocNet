using System.ComponentModel.DataAnnotations;

namespace SocNet.WebAPI.Models
{
    public class UserSignupModel
    {
        [Required]
        [MinLength(6, ErrorMessage = "Minimum 6 characters")]
        [MaxLength(30, ErrorMessage = "Maximum 30 characters")]
        public string Username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Minimum 8 characters")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters")]
        public string Password { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Maximum 30 characters")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Maximum 20 characters")]
        public string LastName { get; set; }
    }
}
