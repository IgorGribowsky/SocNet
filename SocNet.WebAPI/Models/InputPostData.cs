using System.ComponentModel.DataAnnotations;

namespace SocNet.WebAPI.Models;

public class InputPostData
{
    [Required]
    public string Content { get; set; }

    public int ParentPostId { get; set; }
}
