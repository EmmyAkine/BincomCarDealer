using Microsoft.AspNetCore.Mvc;
using BincomCarDealer.Data;

namespace BincomCarDealer.Controllers.Api {
    [ApiController]
    [Route("api/[Controller]")]
    public class CarApiController : ControllerBase {

        private readonly AppDbContext _context;

        public CarApiController(AppDbContext context) {
            _context = context;
        }

        // GET /api/car
        [HttpGet]
        public IActionResult GetAll() {
            var cars = _context.CarItems.OrderByDescending(c => c.UploadedAt).ToList();
            if (!cars.Any()) {
                return NoContent();
            }
            return Ok(cars);
        }

        // GET /api/car/id
        [HttpGet("{id}")]
        public IActionResult GetById(int id) {
            var car = _context.CarItems.FirstOrDefault(c => c.Id == id);
            if (car == null) {
                return NotFound(new { message = $"Car with id {id} not found" });
            }
            return Ok(car);
        }

        // GET /api/car/inquiries
        [HttpGet("inquiries")]
        public IActionResult GetInquiries() {
            var inquiries = _context.CarInquiries.OrderByDescending(i => i.SubmittedAt).ToList();
            if (!inquiries.Any()) {
                return NoContent();
            }
            return Ok(inquiries);
        }
    }
}
