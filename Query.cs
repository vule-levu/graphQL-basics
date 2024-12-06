using GraphQL.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphQL
{
    public class Query
    {
        private readonly ApplicationDbContext _context;
        public Query(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<UserWithPosts>> GetUsersWithPosts(List<int> userIds, [Service] ApplicationDbContext context)
        {
            var users = await context.Users
                .Where(u => userIds.Contains(u.Id))
                .Include(u => u.Posts) // Eagerly load posts
                .ToListAsync();

            return users.Select(u => new UserWithPosts
            {
                User = u,
                Posts = u.Posts
            }).ToList();
        }

        public IQueryable<Post> GetPostsByUser(int userId)
        {
            return _context.Posts
                .Where(p => p.UserId == userId);
        }

        public class UserWithPosts
        {
            public required User User { get; set; }
            public required List<Post> Posts { get; set; }
        }        
    }
}
