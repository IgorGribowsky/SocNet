using SocNet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocNet.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SocNet.Services.AuthenticationManaging
{

    public record SignupDto(string Username, string Password, string FirstName, string LastName);

    public class AuthenticationMonoliticService : ICustomAuthenticationService
    {
        private readonly IJwtManagingService _jwtManager;
        private readonly IRepository _repository;
        private readonly IPasswordHasher<UserIdentity> _passwordHasher;

        public AuthenticationMonoliticService(IJwtManagingService jwtManager, IRepository repository, IPasswordHasher<UserIdentity> passwordHasher)
        {
            _jwtManager = jwtManager;
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserIdentity> GetUSerIdentityByCredentialsAsync(string userName, string password)
        {
            var user = await _repository.Query<UserIdentity>().FirstOrDefaultAsync(u => u.UserName == userName);

            if (user is null)
            {
                return null;
            }

            if (!VerifyPassword(user, password))
            {
                return null;
            }

            return user;
        }

        public UserIdentity GetUSerIdentityById(int id)
        {
            var user = _repository.GetById<UserIdentity>(id);

            return user;
        }

        public async Task<int?> ValidateJwtAsync(string token)
        {
            return await Task.Run(() => _jwtManager.ValidateToken(token));
        }

        private bool VerifyPassword(UserIdentity user, string password)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

            return verificationResult == PasswordVerificationResult.Success || verificationResult == PasswordVerificationResult.SuccessRehashNeeded;
        }

        public async Task<UserIdentity> SignUpAsync(SignupDto userData)
        {
            var createdUser = await _repository.CreateAsync<User>(
                new User { 
                    FirstName = userData.FirstName, 
                    SecondName = userData.LastName 
                });

            var hashedPassword = _passwordHasher.HashPassword(new UserIdentity { }, userData.Password);

            var createdIdentity = await _repository.CreateAsync<UserIdentity>(
                new UserIdentity
                {
                    UserName = userData.Username,
                    Password = hashedPassword,
                    UserId = createdUser.Id
                });

            return createdIdentity;
        }

        public async Task<bool> ChechUsernameUniquenessAsync(string username)
        {
            return !await _repository.Query<UserIdentity>().AnyAsync(u => u.UserName == username);
        }

        public bool TryGetUserId(ClaimsPrincipal user, out int userId)
        {
            var userIdStr = user.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (userIdStr is null || !int.TryParse(userIdStr, out userId))
            {
                userId = 0;
                return false;
            }

            return true;
        }
    }
}
