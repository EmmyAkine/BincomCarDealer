using BincomCarDealer.Data;
using BincomCarDealer.DTO;
using BincomCarDealer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BincomCarDealer.Controllers {
    public class AdminController : Controller {


        private readonly IConfiguration _configuration;

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;


        public AdminController(AppDbContext context, IWebHostEnvironment environment, IConfiguration configuration) {
            _context = context;
            _env = environment;
            _configuration = configuration;
        }

        public IActionResult Index() {
            return RedirectToAction("Dashboard");
        }

        //LOGIN..................

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] CreateLoginRequestDto dto) {

            string? expectedUsername = _configuration["AdminSettings:Username"];
            string? expectedPassword = _configuration["AdminSettings:Password"];

            if (string.IsNullOrEmpty(expectedUsername) || string.IsNullOrEmpty(expectedPassword)) {
                ModelState.AddModelError("", "Authentication service is temporarily unavailable. Please check server configuration.");

                ViewBag.Error = "Authentication service is temporarily unavailable.";
                return View(dto);
            }

            if (dto.Username == expectedUsername && dto.Password == expectedPassword) {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, dto.Username)
                };
                var authProperties = new AuthenticationProperties {
                    IsPersistent = false 
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

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
                imagePath = await UploadImageAndReturnImagePath(dto.Image, _env);
            }
            var car = new CarItem {
                Year = dto.Year,
                Make = dto.Make,
                Model = dto.Model,
                Price = dto.Price,
                Mileage = dto.Mileage,
                Description = dto.Description,
                ImagePath = imagePath,
                BodyStyle = dto.BodyStyle
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

            if (car == null) { return NotFound(); }

            if (!string.IsNullOrEmpty(car.ImagePath)) {
                DeleteImageFile(car.ImagePath, _env);
            }

            _context.CarItems.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageCars");
        }   


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditCarDetails(int id) {
            var car = _context.CarItems.FirstOrDefault(c => c.Id == id);
            if(car == null) {
                return NotFound();
            }
            return View(car);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditCarDetails(int id, [FromForm] EditCarDto dto) {
            var car = _context.CarItems.FirstOrDefault(c => c.Id == id);
            if (car == null) {
                return NotFound();
            }

            car.Year = dto.Year;
            car.Make = dto.Make;
            car.Model = dto.Model;
            car.Price = dto.Price;
            car.Mileage = dto.Mileage;
            car.Description = dto.Description;
            car.BodyStyle = dto.BodyStyle;

            if(dto.Image != null && dto.Image.Length > 0) {
                //Delete the old Image file
                if (!string.IsNullOrEmpty(car.ImagePath)) {
                    DeleteImageFile(car.ImagePath, _env);
                }
                //Upload new image file, overwrite old image path to new image path
                car.ImagePath = await UploadImageAndReturnImagePath(dto.Image, _env);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ManageCars");
        }


        //Inquires..............

        [Authorize]
        public IActionResult Inquiries() {
            var inquiries = _context.CarInquiries.OrderByDescending(i => i.SubmittedAt).ToList();
            return View(inquiries);
        }


        private static async Task<string> UploadImageAndReturnImagePath(IFormFile imageFile, IWebHostEnvironment environment) {
            
            ArgumentNullException.ThrowIfNull(imageFile);

            if (imageFile.Length == 0) {
                throw new ArgumentException("Uploaded file cannot be empty.", nameof(imageFile));
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            string uploadsFolder = Environment.GetEnvironmentVariable("HOME") != null
                ? Path.Combine(Environment.GetEnvironmentVariable("HOME")!, "data", "uploads")
                : Path.Combine(environment.WebRootPath, "uploads");

            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) {
                await imageFile.CopyToAsync(stream);
            }

            var imagePath = "/uploads/" + fileName;
            return imagePath;
        }

        private static void DeleteImageFile(string imagePath, IWebHostEnvironment environment) {
            ArgumentException.ThrowIfNullOrEmpty(imagePath);

            string uploadsFolder = Environment.GetEnvironmentVariable("HOME") != null
                ? Path.Combine(Environment.GetEnvironmentVariable("HOME")!, "data", "uploads")
                : Path.Combine(environment.WebRootPath, "uploads");

            string fileName = Path.GetFileName(imagePath);
            string physicalPath = Path.Combine(uploadsFolder, fileName);

            // Fail-safe check: only attempt deletion if the file physically exists on disk
            if (System.IO.File.Exists(physicalPath)) {
                System.IO.File.Delete(physicalPath);
            }
        }
    }
}
