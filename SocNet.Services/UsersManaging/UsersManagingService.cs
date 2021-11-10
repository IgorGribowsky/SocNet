using SocNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Infrastructure.Interfaces;

namespace SocNet.Services.UsersManaging
{
    public class UsersManagingService : IUsersManagingService
    {
        private readonly IRepository _repository;

        public UsersManagingService(IRepository repository)
        {
            _repository = repository;
        }
        public async Task<User> GetByIdAsync(int id)
        {
            var user = await Task.Run(() => _repository.GetById<User>(id));

            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize)
        {
            var skippedUsers = (page - 1) * pageSize;

            var users = await Task.Run(() => _repository.Query<User>().OrderBy(u=>u.Id).Skip(skippedUsers).Take(pageSize));

            return users;
        }
    }
}
