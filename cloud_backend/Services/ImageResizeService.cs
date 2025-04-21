using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

public class ImageResizeService
{
    public async Task<Stream> CompressImageAsync(Stream inputStream, string fileExtension)
    {
        var outputStream = new MemoryStream();
        using var image = await Image.LoadAsync(inputStream);

        fileExtension = fileExtension.ToLower();

        if (fileExtension == ".jpg" || fileExtension == ".jpeg")
        {
            var encoder = new JpegEncoder
            {
                Quality = 80,
            };

            await image.SaveAsJpegAsync(outputStream, encoder);
        }
        else if (fileExtension == ".png")
        {
            var encoder = new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.BestCompression,
                FilterMethod = PngFilterMethod.Adaptive
            };

            await image.SaveAsPngAsync(outputStream, encoder);
        }
        else
        {
            throw new NotSupportedException("Only JPG, JPEG, and PNG formats are supported.");
        }

        outputStream.Position = 0;
        return outputStream;
    }
}
