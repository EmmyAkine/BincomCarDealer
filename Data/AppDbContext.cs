using BincomCarDealer.Models;
using Microsoft.EntityFrameworkCore;

namespace BincomCarDealer.Data {
    public class AppDbContext : DbContext {
        public AppDbContext (DbContextOptions<AppDbContext>  options) : base(options) { }

        public DbSet<CarItem> CarItems { get; set; }
        public DbSet<CarInquiry> CarInquiries { get; set; }
    }
}
