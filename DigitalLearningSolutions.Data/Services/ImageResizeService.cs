namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using Microsoft.AspNetCore.Http;

    public interface IImageResizeService
    {
        public byte[] ResizeProfilePicture(IFormFile formProfileImage);
    }

    public class ImageResizeService : IImageResizeService
    {
        public byte[] ResizeProfilePicture(IFormFile formProfileImage)
        {
            using var memoryStream = new MemoryStream();
            formProfileImage.CopyTo(memoryStream);

            return SquareImageFromMemoryStream(memoryStream, 300);
        }

        private byte[] SquareImageFromMemoryStream(MemoryStream memoryStream, int targetSideLengthPx)
        {
            using var image = Image.FromStream(memoryStream);

            using var squareImage = CropImageToCentredSquare(image);

            using var resizedImage = ResizeSquareImage(squareImage, targetSideLengthPx);

            using var result = new MemoryStream();
            resizedImage.Save(result, ImageFormat.Jpeg);
            return result.ToArray();
        }

        private Image CropImageToCentredSquare(Image image)
        {
            var minSideLength = Math.Min(image.Height, image.Width);

            var returnSquareImage = new Bitmap(minSideLength, minSideLength);
            using var graphics = Graphics.FromImage(returnSquareImage);
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, minSideLength, minSideLength);

            // Calculate offsets as half the difference between the longer and shorter side
            // This crops an equal amount from either side of the longer side of the source image
            // so the resulting square image is centred in the source image
            var topOffset = 0;
            var leftOffset = 0;
            if (image.Height > image.Width)
            {
                topOffset = (image.Height - image.Width) / 2;
            }
            else
            {
                leftOffset = (image.Width - image.Height) / 2;
            }

            graphics.DrawImage(image,
                new Rectangle(0, 0, minSideLength, minSideLength),
                new Rectangle(leftOffset, topOffset, image.Width - leftOffset * 2, image.Height - topOffset * 2),
                GraphicsUnit.Pixel);

            return returnSquareImage;
        }

        private Image ResizeSquareImage(Image image, int sideLengthPx)
        {
            var destRect = new Rectangle(0, 0, sideLengthPx, sideLengthPx);
            var returnImage = new Bitmap(sideLengthPx, sideLengthPx);

            returnImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using var graphics = Graphics.FromImage(returnImage);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var wrapMode = new ImageAttributes();
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

            return returnImage;
        }
    }
}
