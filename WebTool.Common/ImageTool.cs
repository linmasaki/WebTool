using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;


namespace WebTool.Common
{
    public static class ImageTool
    {
        public enum ResizeMode
        {
            //維持原本的圖片比例,以指定寬高中比例較小的一方為基準去做等比例放大或縮小
            //This will resize images to the resolution nearest to the target resolution. Images can become smaller when using this option
            Normal = 1,

            //以指定的寬高來放大或縮小圖片
            //This will stretch an image so it always is the exact dimensions of the target resolution
            Stretch = 2,

            //以指定寬高中比例較大的一方為基準去做等比例放大或縮小後(維持原本的圖片比例),再以指定的寬高以圖片中心點做剪裁
            //This will size an image to the exact dimensions of the target resolution, keeping ratio in mind and cropping parts that can't fit in the picture.
            Crop = 3,

            //以指定寬高中比例較小的一方為基準去做等比例放大或縮小後(維持原本的圖片比例),不足的部分將以空白補足
            //This will size an image to the exact dimensions of the target resolution, keeping ratio in mind and filling up the image
            //with black bars when some parts remain empty.
            Fill = 4,

            //以指定的寬高及位置來對圖片做剪裁
            Designation = 5
        }

        public static Image Resize(Image image, Size targetResolution, ResizeMode resizeMode, int startX, int startY)
        {
            if (targetResolution.Width == 0 && targetResolution.Height == 0)
            {
                return image;
            }

            //System.Drawing
            int sourceWidth = image.Width;
            int sourceHeight = image.Height;

            int targetWidth = targetResolution.Width;
            int targetHeight = targetResolution.Height;

            float ratioWidth = ((float)targetWidth / (float)sourceWidth);
            float ratioHeight = ((float)targetHeight / (float)sourceHeight);

            float ratio = ratioHeight < ratioWidth ? ratioHeight : ratioWidth;

            if (targetResolution.Width == 0 || targetResolution.Height == 0)
            {
                ratio = ratioWidth + ratioHeight;
                resizeMode = ResizeMode.Normal;
            }

            Bitmap newImage = null;

            switch (resizeMode)
            {
                case ResizeMode.Stretch:
                {
                    newImage = new Bitmap(targetWidth, targetHeight);
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(image, 0, 0, targetWidth, targetHeight);
                    }
                    break;
                }
                case ResizeMode.Crop:
                {
                    ratio = ratioHeight > ratioWidth ? ratioHeight : ratioWidth;

                    int destWidth = (int)(sourceWidth * ratio);
                    int destHeight = (int)(sourceHeight * ratio);

                    newImage = new Bitmap(targetWidth, targetHeight);

                    startX = 0;
                    startY = 0;

                    if (destWidth > targetWidth)
                        startX = 0 - ((destWidth - targetWidth) / 2);

                    if (destHeight > targetHeight)
                        startY = 0 - ((destHeight - targetHeight) / 2);

                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(image, startX, startY, destWidth, destHeight);
                    }

                    break;
                }
                case ResizeMode.Fill:
                {
                    int destWidth = (int)(sourceWidth * ratio);
                    int destHeight = (int)(sourceHeight * ratio);

                    startX = 0;
                    startY = 0;

                    if (destWidth < targetWidth)
                        startX = 0 + ((targetWidth - destWidth) / 2);

                    if (destHeight < targetHeight)
                        startY = 0 + ((targetHeight - destHeight) / 2);

                    newImage = new Bitmap(targetWidth, targetHeight);
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(image, startX, startY, destWidth, destHeight);
                    }

                    break;
                }
                case ResizeMode.Designation:
                {
                    newImage = new Bitmap(targetWidth, targetHeight);
                    startX = startX * (-1);
                    startY = startY * (-1);
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(image, startX, startY, sourceWidth, sourceHeight);
                    }
                    break;
                }
                case ResizeMode.Normal:
                default:
                {
                    int destWidth = (int)(sourceWidth * ratio);
                    int destHeight = (int)(sourceHeight * ratio);

                    newImage = new Bitmap(destWidth, destHeight);
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(image, 0, 0, destWidth, destHeight);
                    }

                    break;
                }
            }

            return newImage;
        }

        /// <summary>
        /// 壓縮圖片
        /// </summary>
        /// <param name="self"></param>
        /// <param name="path"></param>
        /// <param name="flag">0~100</param>
        public static void SaveVaryQualityLevel(this Image self, string path, long flag)
        {
            Bitmap bitmap = new Bitmap(self);
            ImageCodecInfo imageCI = GetEncoder(ImageFormat.Jpeg);
            Encoder myEncoder = Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, flag);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bitmap.Save(path, imageCI, myEncoderParameters);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        /// <summary>
        /// Insert Copyright message
        /// </summary>
        /// <param name="imgPhoto">The photograph to watermark</param>
        /// <param name="copyright">Copyright message(Copyright © 2002 - AP Photo/David Zalubowski)</param>
        /// <returns></returns>
        public static Image Watermark(this Image imgPhoto, string copyright)
        {
            if (null == imgPhoto) return null;
            if (string.IsNullOrEmpty(copyright)) return imgPhoto;

            int phWidth = imgPhoto.Width;
            int phHeight = imgPhoto.Height;

            //create a Bitmap the Size of the original photograph
            Bitmap bmPhoto = new Bitmap(phWidth, phHeight, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            //load the Bitmap into a Graphics object 
            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            //Set the rendering quality for this Graphics object
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;

            //Draws the photo Image object at original size to the graphics object.
            grPhoto.DrawImage(
                imgPhoto, // Photo Image object
                new Rectangle(0, 0, phWidth, phHeight), // Rectangle structure
                0, // x-coordinate of the portion of the source image to draw. 
                0, // y-coordinate of the portion of the source image to draw. 
                phWidth, // Width of the portion of the source image to draw. 
                phHeight, // Height of the portion of the source image to draw. 
                GraphicsUnit.Pixel); // Units of measure 

            //-------------------------------------------------------
            //to maximize the size of the Copyright message we will 
            //test multiple Font sizes to determine the largest posible 
            //font we can use for the width of the Photograph
            //define an array of point sizes you would like to consider as possiblities
            //-------------------------------------------------------
            //int[] sizes = new int[] { 16, 14, 12, 10, 8, 6, 4 };
            int[] sizes = new int[] { 18, 14, 10, 8, 6 };

            Font crFont = null;
            SizeF crSize = new SizeF();

            //Loop through the defined sizes checking the length of the Copyright string
            //If its length in pixles is less then the image width choose this Font size.
            for (int i = 0; i < sizes.Length; i++)
            {
                //set a Font object to Arial (i)pt, Bold
                crFont = new Font("arial", sizes[i], FontStyle.Bold);
                //Measure the Copyright string in this Font
                crSize = grPhoto.MeasureString(copyright, crFont);

                if (crSize.Width < (phWidth / 3))
                    break;
            }

            //Since all photographs will have varying heights, determine a 
            //position 5% from the bottom of the image
            int yPixlesFromBottom = (int)(phHeight * .05);

            //Now that we have a point size use the Copyrights string height 
            //to determine a y-coordinate to draw the string of the photograph
            float yPosFromBottom = ((phHeight - yPixlesFromBottom) - (crSize.Height / 2));

            //Determine its x-coordinate by calculating the center of the width of the image
            //float xCenterOfImg = (phWidth / 2);
            float xLeftOfImg = phWidth - (crSize.Width / 2) - (crSize.Height / 2);

            //Define the text layout by setting the text alignment to centered
            StringFormat StrFormat = new StringFormat();
            StrFormat.Alignment = StringAlignment.Center;

            //define a Brush which is semi trasparent black (Alpha set to 153)
            SolidBrush semiTransBrush2 = new SolidBrush(Color.FromArgb(153, 0, 0, 0));

            //Draw the Copyright string
            grPhoto.DrawString(copyright, //string of text
                               crFont, //font
                               semiTransBrush2, //Brush
                               new PointF(xLeftOfImg + 1, yPosFromBottom + 1), //Position
                               StrFormat);

            //define a Brush which is semi trasparent white (Alpha set to 153)
            SolidBrush semiTransBrush = new SolidBrush(Color.FromArgb(153, 255, 255, 255));

            //Draw the Copyright string a second time to create a shadow effect
            //Make sure to move this text 1 pixel to the right and down 1 pixel
            grPhoto.DrawString(copyright, //string of text
                               crFont, //font
                               semiTransBrush, //Brush
                               new PointF(xLeftOfImg + 1, yPosFromBottom), //Position
                               StrFormat);

            grPhoto.Dispose();

            try
            {
                return new Bitmap(bmPhoto);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                bmPhoto.Dispose();
            }
        }

        public static Image Resize(this Image self, int width, int height, ResizeMode resizeMode = ResizeMode.Normal, int startX = 0, int startY = 0)
        {
            return null == self ? null : Resize(self, new Size(width, height), resizeMode, startX, startY);
        }

        public static bool Resize(string sourceFile, string targetFile, int width, int height)
        {
            if (!File.Exists(sourceFile)) return false;
            try
            {
                var image = Image.FromFile(sourceFile);
                image.Resize(width, height).Save(targetFile, ImageFormat.Jpeg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CreateCache(string photoPath, string cachePath, int width, int height, string copyright = null)
        {
            try
            {
                Image.FromFile(photoPath).Resize(width, height).Watermark(copyright).Save(cachePath, ImageFormat.Jpeg);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
