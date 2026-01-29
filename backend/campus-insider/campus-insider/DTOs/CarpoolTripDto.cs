namespace campus_insider.DTOs
{
    public class CarpoolTripDto
    {
        public int Id { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public UserDto Driver { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
