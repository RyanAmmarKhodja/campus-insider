namespace campus_insider.DTOs
{
    public class EquipmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public UserDto Owner { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
