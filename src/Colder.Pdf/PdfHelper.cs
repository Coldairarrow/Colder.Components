using PDFiumSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace Colder.Pdf;

/// <summary>
/// 
/// </summary>
public static class PdfHelper
{
    /// <summary>
    /// pdf转jpg图片
    /// </summary>
    /// <param name="pdfBytes"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static byte[] Pdf2Jpg(byte[] pdfBytes, int width = 1280, int height = 850)
    {
        using var pdfDocument = new PdfDocument(pdfBytes);
        var firstPage = pdfDocument.Pages[0];

        using var pageBitmap = new PDFiumBitmap(width, height, true);

        firstPage.Render(pageBitmap);

        var image = Image.Load(pageBitmap.AsBmpStream());

        // Set the background to white, otherwise it's black. https://github.com/SixLabors/ImageSharp/issues/355#issuecomment-333133991
        image.Mutate(x => x.BackgroundColor(Color.White));

        using var ms = new MemoryStream();
        image.Save(ms, new JpegEncoder());

        return ms.ToArray();
    }
}
