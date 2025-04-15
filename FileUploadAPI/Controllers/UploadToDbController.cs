using FileUploadAPI.Data;
using FileUploadAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FileUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UploadToDbController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UploadToDbController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> GetALlImages()
        {
            var images = await _context.Images.ToListAsync();
            images.ForEach(x =>
            {
                x.FilePath = $"{Request.Scheme}://{Request.Host}/Images/{x.FileName}";
            });
            return Ok(images);
        }

        [HttpPost]
        [Route("file")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file found");

            var uploadPath = Path.Combine(_env.WebRootPath, "Images");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            //var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif" };
            //if (!allowedTypes.Contains(file.ContentType))
            //    return BadRequest("Invalid file type");
            var extenstions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            string extension = Path.GetExtension(file.FileName);
            if (!extenstions.Contains(extension))
                return BadRequest("Invalid file type");
            // Check file size (5MB limit)
            if (file.Length > 5 * 1024 * 1024) // 5MB
                return BadRequest("File size exceeds the limit of 5MB");
            // Generate a unique file name
            string fileName = Guid.NewGuid().ToString() + extension;

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageModel = new ImageModel()
            {
                FileName = fileName,
                FilePath = $"/Images/{fileName}"
            };
            await _context.Images.AddAsync(imageModel);
            await _context.SaveChangesAsync();

            var fileUrl = $"{Request.Scheme}://{Request.Host}/Images/{fileName}";
            return Ok(new { filePath = fileUrl });
        }
    }
}
