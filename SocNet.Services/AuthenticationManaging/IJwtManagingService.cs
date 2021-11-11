using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Services.AuthenticationManaging
{
    public interface IJwtManagingService
    {
        public string CreateToken(UserIdentity user);

        public int? ValidateToken(string token);
    }
}
