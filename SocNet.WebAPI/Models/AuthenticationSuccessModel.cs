using System.ComponentModel.DataAnnotations;

namespace SocNet.WebAPI.Models
{
    public class AuthenticationSuccessModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
