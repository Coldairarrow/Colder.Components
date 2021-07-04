using Colder.Common;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Domo.Console1
{
    class Program
    {
        static void Main(string[] args)
        {
            Image image = Image.FromFile("1.jpg");
            var newImg = ImgHelper.AddText(image, new Rectangle(0, 0, 50, 50), "测试", Color.White);
            newImg.Save("2.jpg", ImageFormat.Jpeg);

            Console.WriteLine("Hello World!");
        }
    }
}
