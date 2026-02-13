using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class PostService
    {
        private readonly AppDbContext _context;

        public PostService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<PostResponseDto>> CreatePost(PostCreateDto dto, long authorId)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return ServiceResult<PostResponseDto>.Fail("Title is required.");

            if (dto.Title.Length > 200)
                return ServiceResult<PostResponseDto>.Fail("Title must be 200 characters or less.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                return ServiceResult<PostResponseDto>.Fail("Content is required.");

            if (dto.Content.Length > 5000)
                return ServiceResult<PostResponseDto>.Fail("Content must be 5000 characters or less.");

            var post = new Post
            {
                AuthorId = authorId,
                Title = dto.Title.Trim(),
                Content = dto.Content.Trim(),
                ImageUrl = dto.ImageUrl?.Trim(),
                Category = dto.Category,
                Tags = dto.Tags?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Reload with author
            var created = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == post.Id);

            return ServiceResult<PostResponseDto>.Ok(MapToDto(created!));
        }

        public async Task<ServiceResult> LikePost(long postId, long userId)
        {
            // Check if already liked
            var existingLike = await _context.PostLikes
                .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

            if (existingLike != null)
                return ServiceResult.Fail("You already liked this post.");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return ServiceResult.Fail("Post not found.");

            _context.PostLikes.Add(new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });

            post.LikeCount++;
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> UnlikePost(long postId, long userId)
        {
            var like = await _context.PostLikes
                .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

            if (like == null)
                return ServiceResult.Fail("You haven't liked this post.");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return ServiceResult.Fail("Post not found.");

            _context.PostLikes.Remove(like);
            post.LikeCount = Math.Max(0, post.LikeCount - 1);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok();
        }

        private PostResponseDto MapToDto(Post post) => new()
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            Category = post.Category,
            Tags = post.Tags?.Split(',').Select(t => t.Trim()).ToList() ?? new List<string>(),
            LikeCount = post.LikeCount,
            CommentCount = post.CommentCount,
            Author = new UserResponseDto
            {
                Id = post.Author.Id,
                FirstName = post.Author.FirstName,
                LastName = post.Author.LastName,
                Email = post.Author.Email,
                Role = post.Author.Role,
                CreatedAt = post.Author.CreatedAt
            },
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };
    }
}