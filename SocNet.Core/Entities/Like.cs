using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Core.Entities
{
    public class Like
    {
        public int PostId { get; set; }

        public int SenderUserId { get; set; }
    }
}
