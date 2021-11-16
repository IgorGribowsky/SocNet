using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Services.AuthenticationManaging
{
    public interface ICustomAuthenticationService
    {
        public Task<int?> ValidateJwtAsync(string token);

        public UserIdentity GetUSerIdentityById(int id);

        public Task<UserIdentity> GetUSerIdentityByCredentialsAsync(string userName, string password);

        public Task<UserIdentity> SignUpAsync(SignupDto userData);

        public Task<bool> ChechUsernameUniquenessAsync(string username);
    }
}
