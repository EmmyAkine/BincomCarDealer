using BincomCarDealer.Data;
using BincomCarDealer.DTO;
using BincomCarDealer.Models;
using Microsoft.AspNetCore.Mvc;

namespace BincomCarDealer.Controllers {
    public class CarsController : Controller {

        private readonly AppDbContext _context;
        public CarsController(AppDbContext context) {
            _context = context;
        }
        public IActionResult Index() {
            var cars = _context.CarItems.OrderByDescending(c => c.UploadedAt).ToList();
            return View(cars);
        }

        public IActionResult Privacy() {
            return View();
        }

        public IActionResult Details(int id) {
            //Console.WriteLine("I just ran!!! Details");

            var car = _context.CarItems.FirstOrDefault(c => c.Id == id);
            if (car == null) {
                return NotFound();
            }
            return View(car);
        }

        // POST /Cars/Inquire — submit an inquiry
        [HttpPost]
        public async Task<IActionResult> Inquire([FromForm] CreateInquiryFormDto inquiryFormDto) {
            var inquiry = new CarInquiry {
                CarId = inquiryFormDto.CarId,
                Name = inquiryFormDto.Name,
                Email = inquiryFormDto.Email,
                Message = inquiryFormDto.Message
            };

            _context.CarInquiries.Add(inquiry);
            await _context.SaveChangesAsync();

            TempData["InquirySuccess"] = true;
            return RedirectToAction("Details", new { id = inquiryFormDto.CarId });
        }
    }
}
