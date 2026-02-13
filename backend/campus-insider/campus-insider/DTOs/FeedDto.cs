namespace campus_insider.DTOs
{
    // Feed wrapper
    public class FeedResponseDto
    {
        public List<FeedItemDto> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }

    // Polymorphic feed item
    public class FeedItemDto
    {
        public string Type { get; set; } = string.Empty; // EQUIPMENT, CARPOOL, POST
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public object Content { get; set; } = null!; // EquipmentDto, CarpoolDto, or PostDto
        public double Priority { get; set; }
    }

    // Post DTOs
    public class PostCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = "DISCUSSION";
        public string? Tags { get; set; }
    }

    public class PostUpdateDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
    }

    public class PostResponseDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public UserResponseDto Author { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsLikedByCurrentUser { get; set; } // Set in controller
    }

    public class CommentDto
    {
        public long Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public UserResponseDto User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

   
}