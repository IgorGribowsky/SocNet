using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Core.Entities
{
    public class UserIdentity
    {
        public int Id { get; init; }

        public int UserId { get; set; }

        public string UserName { get; set; }
        
        public string Password { get; set; }

    }
}
