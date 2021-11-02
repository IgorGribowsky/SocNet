using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Core.Entities
{
    class Repost
    {
        public int Id { get; private set; }

        public int RepostedUserId { get; set; }

        public int PostId { get; set; }
    }
}
