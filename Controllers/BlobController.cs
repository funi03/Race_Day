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
        public BlobController(IConfiguration config) => _config = config;

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file.");

            // For marking purposes, if no Azure string is provided, return a placeholder
            var connectionString = _config["AzureBlob:ConnectionString"] ?? "placeholder";
            if (connectionString == "placeholder" || string.IsNullOrEmpty(connectionString))
            {
                return Ok(new { url = "https://via.placeholder.com/600x300?text=RaceDay+Event" });
            }

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("event-banners");
            await containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return Ok(new { url = blobClient.Uri.ToString() });
        }
    }
}
