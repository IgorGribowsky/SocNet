using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Services.AuthenticationManaging
{
    public interface IUserValidationService
    {
        public int? ValidateJwt(string token);
        public UserIdentity GetUSerIdentityById(int id);
    }
}
