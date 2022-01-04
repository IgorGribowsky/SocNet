using SocNet.Core.Entities;

namespace SocNet.Services.AuthenticationManaging;

public interface IJwtManagingService
{
    public string CreateToken(UserIdentity user);

    public int? ValidateToken(string token);
}
