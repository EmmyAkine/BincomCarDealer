namespace BincomCarDealer.Models {
    public class CarInquiry {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
