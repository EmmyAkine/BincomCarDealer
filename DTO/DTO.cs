using System.ComponentModel.DataAnnotations;

namespace BincomCarDealer.DTO {
    public class CreateCarUploadDto {
        [Required]
        public string Make { get; set; } = "";

        [Required]
        public string Model { get; set; } = "";

        [Range(1900, 2027)]
        public int Year { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required]
        public string BodyStyle { get; set; } = ""; 

        public string Description { get; set; } = "";

        [Required]
        public required IFormFile Image { get; set; }
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
