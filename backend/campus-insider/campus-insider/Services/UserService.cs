using campus_insider.Data;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetByIdAsync(long id)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            await _context.Users
            .Where(e => e.Id == user.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.FirstName, user.FirstName)
                .SetProperty(e => e.LastName, user.LastName)
                .SetProperty(e => e.Email, user.Email)
                .SetProperty(e => e.Password, user.Password)
            );

            return user;

        }

    }

}

