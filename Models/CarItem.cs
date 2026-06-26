using System.ComponentModel.DataAnnotations.Schema;

namespace BincomCarDealer.Models {
    public class CarItem {
        public int Id { get; set; }

        // --- Main Car Identity ---
        public int Year { get; set; }
        public string Make { get; set; } = "";        // e.g., "Toyota"
        public string Model { get; set; } = "";       // e.g., "Camry"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Mileage { get; set; }

        // --- Visuals & Marketing ---
        public string ImagePath { get; set; } = "";    // Main Image
        public string Description { get; set; } = ""; // Colors, features, and warranty can just be written here!

        // --- Body Type ---
        public string BodyStyle { get; set; } = "";     // e.g., "SUV", "Sedan", "Truck"


        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
