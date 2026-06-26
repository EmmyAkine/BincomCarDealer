using BincomCarDealer.Data;
using BincomCarDealer.DTO;
using BincomCarDealer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace BincomCarDealer.Controllers {
    public class AdminController : Controller {

        private const string AdminUsername = "admin";
        private const string AdminPassword = "Bincom2026!";

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;


        public AdminController(AppDbContext context, IWebHostEnvironment environment) {
            _context = context;
            _env = environment;
        }

        public IActionResult Index() {
            return RedirectToAction("Login");
        }

        //LOGIN..................

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] CreateLoginRequestDto dto) {
            if (dto.Username == AdminUsername && dto.Password == AdminPassword) {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, dto.Username)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        //Dashboard..............

        [Authorize]
        public IActionResult Dashboard() {
            return View();
        }

        //Upload Car ..............
        [Authorize]
        [HttpGet]
        public IActionResult Upload() {
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload(CreateCarUploadDto dto) {
            string imagePath = "";

            if (dto.Image != null && dto.Image.Length > 0) {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder)) {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create)) {
                    await dto.Image.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
            }
            var car = new CarItem {
                Name = dto.Name,
                Brand = dto.Brand,
                Price = dto.Price,
                Description = dto.Description,
                ImagePath = imagePath
            };

            _context.CarItems.Add(car);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageCars");
        }

        //Manage Cars ...................
        [Authorize]
        public IActionResult ManageCars() {
            var cars = _context.CarItems.OrderByDescending(c => c.UploadedAt).ToList();
            return View(cars);
        }

        [Authorize]
        public async Task<IActionResult> DeleteCar(int id) {
            var car = _context.CarItems.FirstOrDefault(c => c.Id == id);
            if (car != null) {
                var filePath = car.ImagePath;
                if (System.IO.File.Exists(filePath)) {
                    System.IO.File.Delete(filePath);
                }
                _context.CarItems.Remove(car);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageCars");
        }

        [Authorize]
        public IActionResult Inquiries() {
            var inquiries = _context.CarInquiries.OrderByDescending(i => i.SubmittedAt).ToList();
            return View(inquiries);
        }
    }
}
