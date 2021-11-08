using SocNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocNet.Services.UsersManaging
{
    public class FakeUsersManagingService : IUsersManagingService
    {
        private readonly List<User> _fakeUsers;

        public FakeUsersManagingService()
        {
            _fakeUsers = new List<User>
            {
                new User {Id = 1, FirstName = "Alex", SecondName = "Gribowski"},
                new User {Id = 2, FirstName = "Schmalex", SecondName = "Libowski"},
                new User {Id = 3, FirstName = "Bebra", SecondName = "Pupa"},
                new User {Id = 4, FirstName = "Shurepa", SecondName = "Lupa"}
            };
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var requestedUser = _fakeUsers.FirstOrDefault(u => u.Id == id);

            return requestedUser;
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize)
        {
            var skippedUsers = (page - 1) * pageSize;

            var users = _fakeUsers.Skip(skippedUsers).Take(pageSize);

            return users;
        }
    }
}
