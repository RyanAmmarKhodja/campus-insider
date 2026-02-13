// Models/Post.cs
namespace campus_insider.Models
{
    public class Post
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }

        // Content
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        // Categorization
        public string Category { get; set; } = string.Empty; // ANNOUNCEMENT, DISCUSSION, EVENT, TIP
        public string? Tags { get; set; } // Comma-separated: "sports,equipment,outdoors"

        // Engagement
        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;

        // State
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User Author { get; set; } = null!;
        public List<PostLike> Likes { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();
    }

    public class PostLike
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Post Post { get; set; } = null!;
        public User User { get; set; } = null!;
    }

    public class Comment
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public Post Post { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}