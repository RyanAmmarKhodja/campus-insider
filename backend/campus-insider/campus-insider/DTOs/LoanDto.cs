namespace campus_insider.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public EquipmentDto Equipment { get; set; }
        public UserDto Borrower { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt {  get; set; }
    }
}
