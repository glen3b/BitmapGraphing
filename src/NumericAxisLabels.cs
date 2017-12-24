using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Brushes;
using SixLabors.ImageSharp.Drawing.Pens;
using SixLabors.Primitives;

namespace BitmapGraphing
{
    public abstract class NumericAxisLabels : IGraphLayer
    {
        public Axis Axis { get; set; }

        public NumericAxisLabels(Axis axis)
        {
            Axis = axis;
        }

        public NumericAxisLabels(Axis axis, Grid grid) : this(axis)
        {
            TickDistance = (axis.IsHorizontal ? grid.LineDistanceHorizontal : grid.LineDistanceVertical) ?? 1;
        }

        public bool EnableForZero { get; set; } = false;

        // TODO this doesn't lend itself nicely to log scales, unless the log is abstracted away from the grid
        /// <summary>
        /// The distance, in graph units, between adjacent labels.
        /// </summary>
        public float TickDistance { get; set; } = 1;

        /// <summary>
        /// Generates a bitmap label for an axis tickmark.
        /// Will be rendered at the relevant position on the axis, so any offset must be applied within the image using transparency.
        /// </summary>
        protected abstract Image<Rgba32> GenerateLabel(float dataCoordinate, GraphicsOptions suppliedOptions);

        public void Render(GraphBuilder context, IImageProcessingContext<Rgba32> renderContext, GraphicsOptions options)
        {
            PointF originPixelPos = (context.GridRegion.Position() + context.Origin);

            if (Axis.IsHorizontal)
            {
                float leftBound = context.GridRegion.Left;
                float rightBound = context.GridRegion.Right;

                float xData = EnableForZero ? 0 : TickDistance;
                float x = originPixelPos.X + context.ToPixelsHorizontal(xData);

                // doesn't lend itself to log scales...
                float pixelTick = context.ToPixelsHorizontal(TickDistance);

                while (x < rightBound)
                {
                    Image<Rgba32> generatedLabel = GenerateLabel(xData, options);
                    renderContext.DrawImage(generatedLabel, generatedLabel.Size(), new Point((int)x, (int)originPixelPos.Y), options);

                    x += pixelTick;
                    xData += TickDistance;
                }

                xData = -TickDistance;
                x = originPixelPos.X + context.ToPixelsHorizontal(xData);


                while (x > leftBound)
                {

                    x -= pixelTick;
                    xData -= TickDistance;
                }
            }
            else
            {
                float topBound = context.GridRegion.Left;
                float bottomBound = context.GridRegion.Right;

                float yData = EnableForZero ? 0 : TickDistance;
                float y = originPixelPos.Y + context.ToPixelsVertical(yData);

                // doesn't lend itself to log scales...
                float pixelTick = context.ToPixelsVertical(TickDistance);

                while (y < bottomBound)
                {
                    Image<Rgba32> generatedLabel = GenerateLabel(yData, options);
                    renderContext.DrawImage(generatedLabel, generatedLabel.Size(), new Point((int)originPixelPos.X, (int)y), options);

                    y += pixelTick;
                    yData += TickDistance;
                }

                yData = -TickDistance;
                y = originPixelPos.Y + context.ToPixelsVertical(yData);


                while (y > topBound)
                {
                    Image<Rgba32> generatedLabel = GenerateLabel(yData, options);
                    renderContext.DrawImage(generatedLabel, generatedLabel.Size(), new Point((int)originPixelPos.X, (int)y), options);

                    y -= pixelTick;
                    yData -= TickDistance;
                }
            }
        }
    }
}