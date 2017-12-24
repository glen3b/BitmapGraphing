using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Brushes;
using SixLabors.ImageSharp.Drawing.Pens;
using SixLabors.Primitives;

namespace BitmapGraphing
{
    public class Grid : IGraphLayer
    {
        // null = don't render
        /// <summary>Distance, in graph units, between grid lines perpendicular to (and aligned with) the horizontal axis.</summary>
        public float? LineDistanceHorizontal { get; set; } = 1;
        /// <summary>Distance, in graph units, between grid lines perpendicular to (and aligned with) the vertical axis.</summary>
        public float? LineDistanceVertical { get; set; } = 1;
        /// <summary>
        /// Gets or sets the interval, in number of gridlines (0 disables), for a visually distinct Y-axis gridline.
        /// </summary>
        public int MajorVerticalGridLineInterval { get; set; } = 5;
        /// <summary>
        /// Gets or sets the interval, in number of gridlines (0 disables), for a visually distinct X-axis gridline.
        /// </summary>
        public int MajorHorizontalGridLineInterval { get; set; } = 5;

        public IPen<Rgba32> VerticalPen { get; set; } = new Pen<Rgba32>(new SolidBrush<Rgba32>(Rgba32.Gray), 1);
        public IPen<Rgba32> MajorVerticalPen { get; set; } = new Pen<Rgba32>(new SolidBrush<Rgba32>(Rgba32.Gray), 2);
        public IPen<Rgba32> HorizontalPen { get; set; } = new Pen<Rgba32>(new SolidBrush<Rgba32>(Rgba32.Gray), 1);
        public IPen<Rgba32> MajorHorizontalPen { get; set; } = new Pen<Rgba32>(new SolidBrush<Rgba32>(Rgba32.Gray), 2);

        public static IBrush<Rgba32> CreateDottedBrush(Rgba32 foreColor, bool vertical = false) => new PatternBrush<Rgba32>(foreColor, Rgba32.Transparent, vertical ? new bool[,] { { true }, { false } } : new bool[,] { { true, false } });
        public static IBrush<Rgba32> CreateDashedBrush(Rgba32 foreColor, bool vertical = false) => new PatternBrush<Rgba32>(foreColor, Rgba32.Transparent, vertical ? new bool[,] { { true }, { true }, { false } } : new bool[,] { { true, true, false } });

        public void Render(GraphBuilder context, IImageProcessingContext<Rgba32> renderContext, GraphicsOptions renderOptions)
        {
            // top left is renderer's origin point

            if (LineDistanceVertical.HasValue)
            {
                float interval = context.ToPixelsVertical(LineDistanceVertical.Value);
                int gridLineCt = 0;
                Action<float> loopBody = y =>
                {
                    renderContext.DrawLines(MajorVerticalGridLineInterval > 0 && gridLineCt % MajorVerticalGridLineInterval == 0 ? MajorVerticalPen : VerticalPen, new PointF[] {
                        new PointF(context.GridRegion.Left, y),
                        new PointF(context.GridRegion.Right, y)
                    }, renderOptions);
                    gridLineCt++;
                };
                // 2 loops so we're guaranteed to be aligned to the origin and X-axis
                // note that major grid lines will overlap axes
                for (float y = context.GridRegion.Top + context.Origin.Y; y < context.GridRegion.Bottom; y += interval)
                {
                    loopBody(y);
                }
                gridLineCt = 0;
                for (float y = context.GridRegion.Top + context.Origin.Y; y > context.GridRegion.Top; y -= interval)
                {
                    loopBody(y);
                }
            }
            if (LineDistanceHorizontal.HasValue)
            {
                float interval = context.ToPixelsHorizontal(LineDistanceHorizontal.Value);
                int gridLineCt = 0;
                Action<float> loopBody = x =>
                {
                    renderContext.DrawLines(MajorHorizontalGridLineInterval > 0 && gridLineCt % MajorHorizontalGridLineInterval == 0 ? MajorHorizontalPen : HorizontalPen, new PointF[] {
                        new PointF(x, context.GridRegion.Top),
                        new PointF(x, context.GridRegion.Bottom)
                    }, renderOptions);
                    gridLineCt++;
                };

                // 2 loops so we're guaranteed to be aligned to the origin and Y-axis
                for (float x = context.GridRegion.Left + context.Origin.X; x < context.GridRegion.Right; x += interval)
                {
                    loopBody(x);
                }
                gridLineCt = 0;
                for (float x = context.GridRegion.Left + context.Origin.X; x > context.GridRegion.Left; x -= interval)
                {
                    loopBody(x);
                }
            }
        }
    }
}