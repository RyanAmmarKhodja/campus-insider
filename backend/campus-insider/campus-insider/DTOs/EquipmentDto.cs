namespace campus_insider.DTOs
{
   
    public class EquipmentCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class EquipmentUpdateDto
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
    }

    public class EquipmentResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long OwnerId { get; set; }
        public string? OwnerName { get; set; } // Add this for feed display
        public DateTime CreatedAt { get; set; }
    }
}
