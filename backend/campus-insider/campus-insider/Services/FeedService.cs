using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class FeedService
    {
        private readonly AppDbContext _context;

        public FeedService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FeedResponseDto> GetFeed(long userId, int page = 1, int pageSize = 20)
        {
            // Define how many items of each type to show
            // Total: pageSize items distributed across content types
            int equipmentCount = (int)(pageSize * 0.3);  // 30% equipment
            int carpoolCount = (int)(pageSize * 0.4);    // 40% carpools
            int postCount = (int)(pageSize * 0.3);       // 30% posts

            // Fetch each content type in parallel for performance
            var equipmentTask = GetRecentEquipment(equipmentCount);
            var carpoolTask = GetUpcomingCarpools(carpoolCount);
            var postTask = GetRecentPosts(postCount);

            await Task.WhenAll(equipmentTask, carpoolTask, postTask);

            // Combine and sort by timestamp
            var feedItems = new List<FeedItemDto>();

            feedItems.AddRange(equipmentTask.Result.Select(e => new FeedItemDto
            {
                Type = "EQUIPMENT",
                Id = e.Id,
                Timestamp = e.CreatedAt,
                Content = e,
                Priority = CalculateEquipmentPriority(e)
            }));

            feedItems.AddRange(carpoolTask.Result.Select(c => new FeedItemDto
            {
                Type = "CARPOOL",
                Id = c.Id,
                Timestamp = c.CreatedAt,
                Content = c,
                Priority = CalculateCarpoolPriority(c)
            }));

            feedItems.AddRange(postTask.Result.Select(p => new FeedItemDto
            {
                Type = "POST",
                Id = p.Id,
                Timestamp = p.CreatedAt,
                Content = p,
                Priority = CalculatePostPriority(p)
            }));

            // Sort by priority first, then by timestamp
            var sortedItems = feedItems
                .OrderByDescending(item => item.Priority)
                .ThenByDescending(item => item.Timestamp)
                .Take(pageSize)
                .ToList();

            return new FeedResponseDto
            {
                Items = sortedItems,
                Page = page,
                PageSize = pageSize,
                TotalItems = sortedItems.Count
            };
        }

        #region --- Content Fetchers ---

        private async Task<List<EquipmentResponseDto>> GetRecentEquipment(int count)
        {
            var equipment = await _context.Equipment
                .AsNoTracking()
                .Include(e => e.Owner)
                .Where(e => e.CreatedAt >= DateTime.UtcNow.AddDays(-7)) // Last 7 days
                .OrderByDescending(e => e.CreatedAt)
                .Take(count)
                .ToListAsync();

            return equipment.Select(e => new EquipmentResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                Category = e.Category,
                Description = e.Description,
                OwnerId = e.OwnerId,
                OwnerName = $"{e.Owner.FirstName} {e.Owner.LastName}",
                CreatedAt = e.CreatedAt
            }).ToList();
        }

        private async Task<List<CarpoolResponseDto>> GetUpcomingCarpools(int count)
        {
            var now = DateTime.UtcNow;
            var carpools = await _context.CarpoolTrips
                .AsNoTracking()
                .Include(c => c.Driver)
                .Include(c => c.Passengers)
                .ThenInclude(p => p.User)
                .Where(c =>
                    c.Status == "PENDING" &&
                    c.AvailableSeats > 0 &&
                    c.DepartureTime >= now &&
                    c.DepartureTime <= now.AddDays(7)) // Next 7 days
                .OrderBy(c => c.DepartureTime)
                .Take(count)
                .ToListAsync();

            return carpools.Select(c => new CarpoolResponseDto
            {
                Id = c.Id,
                Departure = c.Departure,
                Destination = c.Destination,
                DepartureTime = c.DepartureTime,
                Status = c.Status,
                AvailableSeats = c.AvailableSeats,
                TotalSeats = c.AvailableSeats + c.Passengers.Count,
                Driver = new UserResponseDto
                {
                    Id = c.Driver.Id,
                    FirstName = c.Driver.FirstName,
                    LastName = c.Driver.LastName,
                    Email = c.Driver.Email,
                    Role = c.Driver.Role,
                    CreatedAt = c.Driver.CreatedAt
                },
                Passengers = c.Passengers.Select(p => new UserResponseDto
                {
                    Id = p.User.Id,
                    FirstName = p.User.FirstName,
                    LastName = p.User.LastName,
                    Email = p.User.Email,
                    Role = p.User.Role,
                    CreatedAt = p.User.CreatedAt
                }).ToList(),
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        private async Task<List<PostResponseDto>> GetRecentPosts(int count)
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Author)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();

            return posts.Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                Category = p.Category,
                Tags = p.Tags?.Split(',').Select(t => t.Trim()).ToList() ?? new List<string>(),
                LikeCount = p.LikeCount,
                CommentCount = p.CommentCount,
                Author = new UserResponseDto
                {
                    Id = p.Author.Id,
                    FirstName = p.Author.FirstName,
                    LastName = p.Author.LastName,
                    Email = p.Author.Email,
                    Role = p.Author.Role,
                    CreatedAt = p.Author.CreatedAt
                },
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
        }

        #endregion

        #region --- Priority Calculation (Ranking Algorithm) ---

        private double CalculateEquipmentPriority(EquipmentResponseDto equipment)
        {
            // Newer equipment gets higher priority
            var daysSinceCreated = (DateTime.UtcNow - equipment.CreatedAt).TotalDays;
            var recencyScore = Math.Max(0, 7 - daysSinceCreated); // Score 0-7

            // TODO: Add more factors:
            // - Category popularity
            // - Owner reputation
            // - Number of past successful loans

            return recencyScore;
        }

        private double CalculateCarpoolPriority(CarpoolResponseDto carpool)
        {
            // Carpools departing soon get higher priority
            var hoursUntilDeparture = (carpool.DepartureTime - DateTime.UtcNow).TotalHours;
            var urgencyScore = hoursUntilDeparture < 24 ? 10 :
                               hoursUntilDeparture < 72 ? 7 :
                               5;

            // More available seats = higher priority
            var availabilityScore = carpool.AvailableSeats * 2;

            // TODO: Add more factors:
            // - Geographic proximity to user
            // - Driver rating
            // - Route popularity

            return urgencyScore + availabilityScore;
        }

        private double CalculatePostPriority(PostResponseDto post)
        {
            // Engagement score
            var engagementScore = (post.LikeCount * 1.5) + (post.CommentCount * 2);

            // Recency score
            var hoursSinceCreated = (DateTime.UtcNow - post.CreatedAt).TotalHours;
            var recencyScore = Math.Max(0, 48 - hoursSinceCreated) / 4; // Score 0-12

            // Announcements get boosted
            var categoryBoost = post.Category == "ANNOUNCEMENT" ? 5 : 0;

            // TODO: Add more factors:
            // - Author reputation
            // - Tag relevance to user interests
            // - Time decay factor

            return engagementScore + recencyScore + categoryBoost;
        }

        #endregion
    }
}