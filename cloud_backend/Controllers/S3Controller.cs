using cloud_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cloud_backend.Controllers
{
    [ApiController]
    [Route("s3")]
    public class S3Controller : ControllerBase
    {
        private readonly S3Service _s3Service;
        private readonly ImageResizeService _imageResizeService;

        public S3Controller(ImageResizeService imageResizeService,S3Service s3Service)
        {
            _imageResizeService = imageResizeService;
            _s3Service = s3Service;
        }

        [Authorize]
        [HttpPost("uploadImage")]
        public async Task<ActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0 )
                return BadRequest(new { message = "Invalid image file." });

            using var inputStream = imageFile.OpenReadStream();
            var fileExtension = Path.GetExtension(imageFile.FileName);

            using var resizedImage = await _imageResizeService.CompressImageAsync(inputStream, fileExtension);

            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var contentType = imageFile.ContentType;

            var imageUrl = await _s3Service.UploadImageToS3Async(resizedImage, contentType, fileName);

            return Ok(new { imageUrl });
        }
    }

}
