using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;

namespace BitmapGraphing
{
    public interface IGraphLayer
    {
        void Render(GraphBuilder builder, IImageProcessingContext<Rgba32> renderContext, GraphicsOptions options);
    }
}
