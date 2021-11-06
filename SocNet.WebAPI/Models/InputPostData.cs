using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SocNet.WebAPI.Models
{
    public class InputPostData
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public int UserId { get; set; }

        public int ParentPostId { get; set; }
    }
}
