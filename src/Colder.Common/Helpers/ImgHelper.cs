using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace Colder.Common
{
    /// <summary>
    /// 图片操作帮助类
    /// </summary>
    public class ImgHelper
    {
        /// <summary>
        /// 从base64字符串读入图片
        /// </summary>
        /// <param name="base64">base64字符串</param>
        /// <returns></returns>
        public static Image GetImgFromBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            Image img = Image.FromStream(memStream);

            return img;
        }

        /// <summary>
        /// 从URL格式的Base64图片获取真正的图片
        /// 即去掉data:image/jpg;base64,这样的格式
        /// </summary>
        /// <param name="base64Url">图片Base64的URL形式</param>
        /// <returns></returns>
        public static Image GetImgFromBase64Url(string base64Url)
        {
            string base64 = GetBase64String(base64Url);

            return GetImgFromBase64(base64);
        }

        /// <summary>
        /// 压缩图片
        /// 注:等比压缩
        /// </summary>
        /// <param name="img">原图片</param>
        /// <param name="width">压缩后宽度</param>
        /// <returns></returns>
        public static Image CompressImg(Image img, int width)
        {
            return CompressImg(img, width, (int)(((double)width) / img.Width * img.Height));
        }

        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="img">原图片</param>
        /// <param name="width">压缩后宽度</param>
        /// <param name="height">压缩后高度</param>
        /// <returns></returns>
        public static Image CompressImg(Image img, int width, int height)
        {
            Bitmap bitmap = new Bitmap(img, width, height);

            return bitmap;
        }

        /// <summary>
        /// 将图片转为base64字符串
        /// 默认使用jpg格式
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns></returns>
        public static string ToBase64String(Image img)
        {
            return ToBase64String(img, ImageFormat.Jpeg);
        }

        /// <summary>
        /// 将图片转为base64字符串
        /// 使用指定格式
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <param name="imageFormat">指定格式</param>
        /// <returns></returns>
        public static string ToBase64String(Image img, ImageFormat imageFormat)
        {
            MemoryStream memStream = new MemoryStream();
            img.Save(memStream, imageFormat);
            byte[] bytes = memStream.ToArray();
            string base64 = Convert.ToBase64String(bytes);

            return base64;
        }

        /// <summary>
        /// 将图片转为base64字符串
        /// 默认使用jpg格式,并添加data:image/jpg;base64,前缀
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <returns></returns>
        public static string ToBase64StringUrl(Image img)
        {
            return "data:image/jpg;base64," + ToBase64String(img, ImageFormat.Jpeg);
        }

        /// <summary>
        /// 将图片转为base64字符串
        /// 使用指定格式,并添加data:image/jpg;base64,前缀
        /// </summary>
        /// <param name="img">图片对象</param>
        /// <param name="imageFormat">指定格式</param>
        /// <returns></returns>
        public static string ToBase64StringUrl(Image img, ImageFormat imageFormat)
        {
            string base64 = ToBase64String(img, imageFormat);

            return $"data:image/{imageFormat.ToString().ToLower()};base64,{base64}";
        }

        /// <summary>
        /// 获取真正的图片base64数据
        /// 即去掉data:image/jpg;base64,这样的格式
        /// </summary>
        /// <param name="base64UrlStr">带前缀的base64图片字符串</param>
        /// <returns></returns>
        public static string GetBase64String(string base64UrlStr)
        {
            string parttern = "^(data:image/.*?;base64,).*?$";

            var match = Regex.Match(base64UrlStr, parttern);
            if (match.Groups.Count > 1)
                base64UrlStr = base64UrlStr.Replace(match.Groups[1].ToString(), "");

            return base64UrlStr;
        }

        /// <summary>
        /// 图片添加文字水印，文字大小自适应
        /// </summary>
        /// <param name="image">图片</param>
        /// <param name="area">文字区域</param>
        /// <param name="text">文字内容</param>
        /// <param name="color">字体颜色</param>
        /// <param name="fontName">字体名称</param>
        /// <returns>添加文字后的Bitmap对象</returns>
        public static Image AddText(Image image, Rectangle area, string text, Color color, string fontName = "微软雅黑")
        {
            int width = image.Width;
            int height = image.Height;
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                // 计算文字区域
                // 区域宽高
                float fontWidth = area.Width;
                float fontHeight = area.Height;
                // 初次估计先用文字区域高度作为文字字体大小，后面再做调整，单位为px
                float fontSize = fontHeight;
                Font font = new Font(fontName, fontSize, GraphicsUnit.Pixel);
                SizeF sf = graph.MeasureString(text, font);
                int times = 0;
                // 调整字体大小以适应文字区域
                if (sf.Width > fontWidth)
                {
                    while (sf.Width > fontWidth)
                    {
                        fontSize -= 0.1f;
                        font = new Font(fontName, fontSize, GraphicsUnit.Pixel);
                        sf = graph.MeasureString(text, font);
                        times++;
                    }
                }
                else if (sf.Width < fontWidth)
                {
                    while (sf.Width < fontWidth)
                    {
                        fontSize += 0.1f;
                        font = new Font(fontName, fontSize, GraphicsUnit.Pixel);
                        sf = graph.MeasureString(text, font);
                        times++;
                    }
                }
                // 最终的得出的字体所占区域一般不会刚好等于实际区域
                // 所以根据两个区域的相差之处再把文字开始位置(左上角定位)稍微调整一下
                var x1 = area.X + ((fontWidth - sf.Width) / 2);
                var y1 = area.Y + ((fontHeight - sf.Height) / 2);
                graph.DrawImage(image, 0, 0, width, height);
                graph.DrawString(text, font, new SolidBrush(color), x1, y1);
                return bmp;
            }
        }
    }
}
