using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Services.UsersManaging
{
    public interface IUsersManagingService
    {
        public Task<User> GetByIdAsync(int id);

        public Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize);
    }
}
