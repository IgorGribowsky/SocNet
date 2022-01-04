using System.Collections.Generic;
using System.Threading.Tasks;
using SocNet.Core.Entities;

namespace SocNet.Services.UsersManaging;

public interface IUsersManagingService
{
    public Task<User> GetByIdAsync(int id);

    public Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize);
}
