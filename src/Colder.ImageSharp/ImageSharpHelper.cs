using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace Colder.ImageSharp;

/// <summary>
///
/// </summary>
public static class ImageSharpHelper
{
    /// <summary>
    /// 等比压缩图片并转为jpg
    /// </summary>
    /// <param name="imageBytes"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static byte[] CompressImage(byte[] imageBytes, int width = 1080)
    {
        var image = Image.Load(imageBytes);
        image.Metadata.ExifProfile = null;

        if (image.Width > width)
        {
            var rate = (double)width / image.Width;
            image.Mutate(x => x.Resize((int)(image.Width * rate), (int)(image.Height * rate)));
        }

        var ms = new MemoryStream();
        image.SaveAsJpeg(ms, new JpegEncoder()
        {
            Quality = 75
        });

        return ms.ToArray();
    }
}