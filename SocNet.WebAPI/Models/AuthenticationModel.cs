using System.ComponentModel.DataAnnotations;

namespace SocNet.WebAPI.Models
{
    public class AuthenticationModel
    {
        [Required(ErrorMessage = "Please enter your username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        public string Password { get; set; }
    }
}
