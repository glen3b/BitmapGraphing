using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.Primitives;

namespace BitmapGraphing
{
    public class GraphBuilder
    {
        /// <summary>
        /// The region, delimited in pixels, that the grid and data occupy.
        /// </summary>
        public RectangleF GridRegion { get; set; }

        /// <summary>
        /// Gets or sets the location of the origin, in pixels, with respect to the position of the GridRegion.
        /// </summary>
        public PointF Origin { get; set; }

        public float PixelsPerGraphUnitVertical { get; set; } = 1;
        public float PixelsPerGraphUnitHorizontal { get; set; } = 1;

        // TODO log scale? that could be implemented data side
        // this function is multiplicative, note it does NOT, for instance, apply the "+GridRegion.Position" transform
        public PointF ToPixels(PointF inGraphUnits)
        {
            return new PointF(ToPixelsHorizontal(inGraphUnits.X), ToPixelsVertical(inGraphUnits.Y));
        }

        public float ToPixelsVertical(float verticalGraphUnits) => verticalGraphUnits * PixelsPerGraphUnitVertical;
        public float ToPixelsHorizontal(float horizontalGraphUnits) => horizontalGraphUnits * PixelsPerGraphUnitHorizontal;

        public float ToGraphUnitsVertical(float verticalPixels) => verticalPixels / PixelsPerGraphUnitVertical;
        public float ToGraphUnitsHorizontal(float horizontalPixels) => horizontalPixels / PixelsPerGraphUnitHorizontal;

        public Grid GridLayer { get; set; } = new Grid();


        public Axis VerticalAxis { get; set; } = new Axis() { IsVertical = true };
        public NumericAxisLabels VerticalAxisLabeler { get; set; }
        public Axis HorizontalAxis { get; set; } = new Axis() { IsHorizontal = true };
        public NumericAxisLabels HorizontalAxisLabeler { get; set; }

        public IList<LinearData> DataSets { get; } = new List<LinearData>();

        public IList<IGraphLayer> AdditionalLayers { get; } = new List<IGraphLayer>();

        public void Render(Image<Rgba32> target) => Render(target, GraphicsOptions.Default);

        public void Render(Image<Rgba32> target, GraphicsOptions renderOptions)
        {
            target.Mutate(context =>
            {
                GridLayer?.Render(this, context, renderOptions);
                VerticalAxis?.Render(this, context, renderOptions);
                VerticalAxisLabeler?.Render(this, context, renderOptions);
                HorizontalAxis?.Render(this, context, renderOptions);
                HorizontalAxisLabeler?.Render(this, context, renderOptions);

                foreach (var dataLayer in DataSets)
                {
                    dataLayer?.Render(this, context, renderOptions);
                }

                foreach (var extraLayer in AdditionalLayers)
                {
                    extraLayer?.Render(this, context, renderOptions);
                }
            });
        }
    }
}
