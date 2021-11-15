using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SocNet.Core.Entities
{
    [Index(nameof(UserName), IsUnique = true)]
    public class UserIdentity
    {
        public int Id { get; init; }

        public int UserId { get; set; }

        public string UserName { get; set; }
        
        public string Password { get; set; }
    }
}
