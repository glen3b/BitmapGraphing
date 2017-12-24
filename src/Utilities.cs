using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.Primitives;

namespace BitmapGraphing
{
    internal static class Utilities
    {
        public static PointF Position(this RectangleF rect)
        {
            return new PointF(rect.X, rect.Y);
        }

        public static Size Size<TPixel>(this Image<TPixel> img) where TPixel : struct, SixLabors.ImageSharp.PixelFormats.IPixel<TPixel> => new Size(img.Width, img.Height);

        // TODO this feels like it could be supported for arbitrary TPixel, since many have alpha
        public static void MutateCropToColored(this Image<Rgba32> img)
        {
            int minColorX = img.Width, maxColorX = 0;
            int minColorY = img.Height, maxColorY = 0;
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    // TODO is there a better way than calling the read accessor every time
                    Rgba32 pix = img[x, y];
                    if (pix.A > 0)
                    {
                        if (x < minColorX) minColorX = x;
                        if (x > maxColorX) maxColorX = x;
                        if (y < minColorY) minColorY = y;
                        if (y > maxColorY) maxColorY = y;
                    }
                }
            }
            img.Mutate(context => context.Crop(new Rectangle(minColorX, minColorY, maxColorX - minColorX, maxColorY - minColorY)));
        }
    }
}