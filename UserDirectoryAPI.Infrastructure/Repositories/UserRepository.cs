using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserDirectoryAPI.Application.Interfaces;
using UserDirectoryAPI.Domain.Entities;
using UserDirectoryAPI.Infrastructure.Data;

namespace UserDirectoryAPI.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<User>> GetAllAsync()
        {            
            _logger.LogInformation("Fetching all users from database");
            return await _context.Users.ToListAsync();          
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Fetching user with ID {UserId}", id);
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created user with ID {UserId}", user.Id);
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated user with ID {UserId}", user.Id);
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted user with ID {UserId}", user.Id);
        }
    }
}
