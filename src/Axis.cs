using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Brushes;
using SixLabors.Primitives;
using SixLabors.Fonts;

namespace BitmapGraphing
{
    public class Axis : IGraphLayer
    {
        public Axis()
        {
        }

        // pixels
        public float LineThickness { get; set; } = 2;
        public IBrush<Rgba32> Brush { get; set; } = new SolidBrush<Rgba32>(Rgba32.Black);
        public bool IsHorizontal { get; set; }
        public bool IsVertical
        {
            get
            {
                return !IsHorizontal;
            }
            set
            {
                IsHorizontal = !value;
            }
        }
        // pixels
        public float ArrowWidth { get; set; } = 4;
        public float ArrowLength { get; set; } = 7;
        public Rgba32 ArrowColor { get; set; } = Rgba32.Black;

        // margins in pixels
        public bool EnablePositiveEndArrow { get; set; } = true;
        public float PositiveEndAxisMargin { get; set; } = 0;
        public bool EnableNegativeEndArrow { get; set; } = true;
        public float NegativeEndAxisMargin { get; set; } = 0;

        public class LabelInfo
        {
            public Rgba32 Color { get; set; } = Rgba32.Black;
            public Font Font { get; set; }
            public string Text { get; set; }
            public PointF Displacement { get; set; }
        }

        // displacement: from positive endpoint of axis
        public LabelInfo EndpointLabel { get; set; }
        // displacement: from center of positive end of axis, positioning works as if positioning the top center of the text
        public LabelInfo TitleLabel { get; set; }

        public void Render(GraphBuilder context, IImageProcessingContext<Rgba32> renderContext, GraphicsOptions options)
        {
            // top and right
            PointF labelEndpoint;
            if (IsVertical)
            {
                PointF endpoint1 = new PointF(context.GridRegion.Left + context.Origin.X, context.GridRegion.Top + PositiveEndAxisMargin + ArrowLength);
                PointF endpoint2 = new PointF(context.GridRegion.Left + context.Origin.X, context.GridRegion.Bottom - NegativeEndAxisMargin - ArrowLength);
                labelEndpoint = endpoint1;

                renderContext.DrawLines(Brush, LineThickness, new PointF[] { endpoint1, endpoint2 });
                if (ArrowWidth > 0 && ArrowLength > 0)
                {
                    if (EnablePositiveEndArrow)
                    {
                        // top, add Y
                        renderContext.FillPolygon(ArrowColor, new PointF[] {
                            endpoint1 - new PointF(0, ArrowLength),
                            endpoint1 + new PointF(-ArrowWidth / 2, 0),
                            endpoint1 + new PointF(ArrowWidth / 2, 0)
                        }, options);
                    }
                    if (EnableNegativeEndArrow)
                    {
                        // bottom, subtract Y
                        renderContext.FillPolygon(ArrowColor, new PointF[] {
                            endpoint2 + new PointF(0, ArrowLength),
                            endpoint2 - new PointF(-ArrowWidth / 2, 0),
                            endpoint2 - new PointF(ArrowWidth / 2, 0)
                        }, options);
                    }
                }

                if (TitleLabel != null)
                {
                    RectangleF textBounds = TextMeasurer.MeasureBounds(TitleLabel.Text, new RendererOptions(TitleLabel.Font));
                    int higherDimension = (int)Math.Ceiling(Math.Max(textBounds.Width, textBounds.Height));
                    using (Image<Rgba32> textRenderImage = new Image<Rgba32>(higherDimension, higherDimension))
                    {
                        textRenderImage.Mutate(tempContext =>
                            tempContext
                                .DrawText(TitleLabel.Text, TitleLabel.Font, TitleLabel.Color, PointF.Empty, options)
                                .Rotate(-90));
                        // TODO can I do this in the same IImageProcessingContext or do I have to stop, let it write, then read, then write
                        textRenderImage.MutateCropToColored();
                        PointF renderPosition = context.GridRegion.Position() + context.Origin + TitleLabel.Displacement - textRenderImage.Size() + new PointF(0, (textRenderImage.Height / 2) - ((context.Origin.Y + context.GridRegion.Top - endpoint1.Y)/2));
                        renderContext.DrawImage(textRenderImage, textRenderImage.Size(), (Point)renderPosition, options);
                    }
                }
            }
            else
            {
                PointF endpoint1 = new PointF(context.GridRegion.Left + NegativeEndAxisMargin + ArrowLength, context.GridRegion.Top + context.Origin.Y);
                PointF endpoint2 = new PointF(context.GridRegion.Right - PositiveEndAxisMargin - ArrowLength, context.GridRegion.Top + context.Origin.Y);
                labelEndpoint = endpoint2;

                renderContext.DrawLines(Brush, LineThickness, new PointF[] { endpoint1, endpoint2 });
                if (ArrowWidth > 0 && ArrowLength > 0)
                {
                    if (EnableNegativeEndArrow)
                    {
                        // left, add X
                        renderContext.FillPolygon(ArrowColor, new PointF[] {
                            endpoint1 - new PointF(ArrowLength, 0),
                            endpoint1 + new PointF(0, -ArrowWidth / 2),
                            endpoint1 + new PointF(0, ArrowWidth / 2)
                        }, options);
                    }
                    if (EnablePositiveEndArrow)
                    {
                        // right, subtract X
                        renderContext.FillPolygon(ArrowColor, new PointF[] {
                            endpoint2 + new PointF(ArrowLength, 0),
                            endpoint2 - new PointF(0, -ArrowWidth / 2),
                            endpoint2 - new PointF(0, ArrowWidth / 2)
                        }, options);
                    }
                }

                if (TitleLabel != null)
                {
                    //var textBounds = TextMeasurer.Measure(TitleLabel.Text, new RendererOptions(TitleLabel.Font));
                    // TODO: why does this work without a textBounds displacement for re-origining?
                    var pos = context.GridRegion.Position() + context.Origin + TitleLabel.Displacement + new PointF(((endpoint2.X - (context.GridRegion.Left + context.Origin.X)) / 2), 0);
                    renderContext.DrawText(TitleLabel.Text, TitleLabel.Font, TitleLabel.Color, pos, options);
                }
            }

            if (EndpointLabel != null)
            {
                renderContext.DrawText(EndpointLabel.Text, EndpointLabel.Font, EndpointLabel.Color, labelEndpoint + EndpointLabel.Displacement, options);
            }
        }
    }
}