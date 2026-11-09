using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

namespace Race_Day.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Organiser")]
    public class BlobController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _environment;

        public BlobController(IConfiguration config, IWebHostEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file.");

            var connectionString = _config["AzureBlob:ConnectionString"];

            // OPTION 1: If you have a real Azure Connection String, use Azure!
            if (!string.IsNullOrEmpty(connectionString) && connectionString != "placeholder")
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("event-banners");
                await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, true);

                return Ok(new { url = blobClient.Uri.ToString() });
            }

            // OPTION 2: Local Fallback (For your Lab PC demonstration)
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadDir = Path.Combine(webRoot, "images");

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"{Request.Scheme}://{Request.Host}/images/{fileName}";
            return Ok(new { url = url });
        }
    }
}