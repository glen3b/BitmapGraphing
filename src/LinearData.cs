using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Brushes;
using SixLabors.ImageSharp.Drawing.Pens;
using SixLabors.Primitives;

namespace BitmapGraphing
{
    public class LinearData : IGraphLayer
    {
        public struct DataPoint
        {
            public double X { get; }
            public double Y { get; }

            public bool BreakLine { get; }

            public DataPoint(double x, double y) : this(x, y, false)
            {

            }

            public DataPoint(double x, double y, bool breakLine)
            {
                X = x;
                Y = y;
                BreakLine = breakLine;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is DataPoint other))
                {
                    return false;
                }
                return Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + X.GetHashCode();
                    hash = hash * 23 + Y.GetHashCode();
                    hash = hash * 23 + BreakLine.GetHashCode();
                    return hash;
                }
            }

            private bool Equals(DataPoint other)
            {
                return other.X == X && other.Y == Y && other.BreakLine == BreakLine;
            }

            public static explicit operator PointF(DataPoint data)
            {
                return new PointF((float)data.X, (float)data.Y);
            }

            public static bool operator ==(DataPoint left, DataPoint right) => left.Equals(right);

            public static bool operator !=(DataPoint left, DataPoint right) => !left.Equals(right);
        }

        // in pixels
        public float DataPointCircleRadius { get; set; } = 2.5f;
        public Rgba32 PointColor { get; set; } = Rgba32.Red;
        public IPen<Rgba32> LinePen { get; set; } = new Pen<Rgba32>(new SolidBrush<Rgba32>(Rgba32.Red), 1);
        // data points in graph units
        public IList<DataPoint> Data { get; } = new List<DataPoint>();

        public void Render(GraphBuilder context, IImageProcessingContext<Rgba32> renderContext, GraphicsOptions options)
        {
            float leftBoundPixel = context.GridRegion.Left;
            float rightBoundPixel = context.GridRegion.Right;
            float topBoundPixel = context.GridRegion.Top;
            float bottomBoundPixel = context.GridRegion.Bottom;

            var data = Data.Select(point => new
            {
                PixelLocation = ((PointF)point) + context.GridRegion.Position() + context.Origin,
                Data = point
            }).Where(point => point.PixelLocation.X > leftBoundPixel && point.PixelLocation.X < rightBoundPixel && point.PixelLocation.Y > topBoundPixel && point.PixelLocation.Y < bottomBoundPixel)
            .OrderBy(point => point.Data.X)
            .ToArray();
            while (data.Length > 0)
            {
                List<PointF> pixels = data.TakeWhile(p => !p.Data.BreakLine).Select(p => p.PixelLocation).ToList();
                if (pixels.Count < data.Length)
                {
                    // add the line-breaking point
                    pixels.Add(data[pixels.Count].PixelLocation);
                }
                if (LinePen != null)
                {
                    renderContext.DrawLines(LinePen, pixels.ToArray(), options);
                }
                if (DataPointCircleRadius > 0)
                {
                    foreach (var pixel in pixels)
                    {
                        renderContext.Fill(PointColor, new SixLabors.Shapes.EllipsePolygon(pixel, DataPointCircleRadius), options);
                    }
                }
                data = data.Skip(pixels.Count).ToArray();
            }
        }
    }
}