using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
        {
            _env = env;
            _logger = logger;
        }
        [HttpPost]
        [Route("file")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file found");
            }

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Invalid file type");

            var filePath = Path.Combine(uploadPath, "test.png");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"Request Scheme: {Request.Scheme}");
            _logger.LogInformation($"Request Host: {Request.Host}");

            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{file.FileName}";
            return Ok(new { filePath = fileUrl });
        }
    }
}
