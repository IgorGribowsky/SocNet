using SocNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Infrastructure.Interfaces;

namespace SocNet.Services.AuthenticationManaging
{
    public class UserValidatorMonolithic : IUserValidationService
    {
        private readonly IJwtManagingService _jwtManager;
        private readonly IRepository _repository;

        public UserValidatorMonolithic(IJwtManagingService jwtManager, IRepository repository)
        {
            _jwtManager = jwtManager;
            _repository = repository;
        }

        public UserIdentity GetUSerIdentityById(int id)
        {
            var user = _repository.GetById<UserIdentity>(id);

            return user;
        }

        public int? ValidateJwt(string token)
        {
            return _jwtManager.ValidateToken(token);
        }
    }
}
