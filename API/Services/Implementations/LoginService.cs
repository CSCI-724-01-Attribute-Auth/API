using API.Data;
using API.Models;
using API.Services.Interfaces;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

// The MovieService class acts as a centralized service layer for managing Movie entities,
//  performing create, retrieve, and validate operations with a clear separation of concerns. 
// The CreateMovie method includes validation to prevent inconsistent crew assignments, ensuring
// the integrity of the movie-crew relationships. This service would likely be used by other parts 
//of the application to access and manage movie data reliably.

namespace API.Services.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly DBContext _dbContext;

        public LoginService(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Login(string userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.UserId == userId);

            if (user != null)
            {
                //Console.WriteLine("{0}, {1}", userId, user.RoleId);
                return JwtTokenGenerator.GenerateToken(userId, user.RoleId);
            }

            throw new UnauthorizedAccessException();
        }
    }
}
