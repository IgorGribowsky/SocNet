using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Core.Entities
{
    public class Post
    {
        public int Id { get; private set; }

        public string Content { get; set; }

        public int UserId { get; set; }

        public int? ParentPostId { get; set; }

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }
    }
}
