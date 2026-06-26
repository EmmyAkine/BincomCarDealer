namespace BincomCarDealer.DTO {
    public class CreateCarUploadDto {
        public string Name { get; set; } = "";
        public string Brand { get; set; } = "";
        public decimal Price { get; set; }
        public string Description { get; set; } = "";
        public required IFormFile Image {  get; set; }
    }

    public class CreateInquiryFormDto {
        public int CarId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class CreateLoginRequestDto {
        public string Username { get; set; } = "";
        public string Password { get; set; } = ""; 
    }


}
