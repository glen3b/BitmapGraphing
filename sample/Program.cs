using System;
using BitmapGraphing;
using SixLabors.ImageSharp;
using SixLabors.Primitives;

namespace sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var graph = new GraphBuilder();
            graph.PixelsPerGraphUnitHorizontal = 20;
            graph.PixelsPerGraphUnitVertical = 20;
            graph.GridRegion = new RectangleF(50, 50, 600, 600);
            graph.Origin = new PointF(300, 300);
            LinearData testData = new LinearData();
            testData.Data.Add(new LinearData.DataPoint(1, 1));
            testData.Data.Add(new LinearData.DataPoint(3, 5));
            testData.Data.Add(new LinearData.DataPoint(5, 6.5));
            graph.DataSets.Add(testData);
            graph.HorizontalAxis.NegativeEndAxisMargin = 20;
            graph.HorizontalAxis.PositiveEndAxisMargin = 20;
            graph.HorizontalAxis.EndpointLabel = new Axis.LabelInfo()
            {
                Displacement = new PointF(0, 5),
                Color = Rgba32.DarkOliveGreen,
                Font = SixLabors.Fonts.SystemFonts.CreateFont("Ubuntu", 12),
                Text = "X"
            };
            graph.HorizontalAxis.TitleLabel = new Axis.LabelInfo()
            {
                Displacement = new PointF(0, 0),
                Color = Rgba32.BlueViolet,
                Font = SixLabors.Fonts.SystemFonts.CreateFont("Ubuntu", 16),
                Text = "Time"
            };
            graph.VerticalAxis.NegativeEndAxisMargin = 20;
            graph.VerticalAxis.PositiveEndAxisMargin = 20;
            graph.VerticalAxis.EndpointLabel = new Axis.LabelInfo()
            {
                Displacement = new PointF(5, 0),
                Color = Rgba32.DarkOliveGreen,
                Font = SixLabors.Fonts.SystemFonts.CreateFont("Ubuntu", 12),
                Text = "Y"
            };
            graph.VerticalAxis.TitleLabel = new Axis.LabelInfo()
            {
                Displacement = new PointF(0, 0),
                Color = Rgba32.BlueViolet,
                Font = SixLabors.Fonts.SystemFonts.CreateFont("Ubuntu", 16),
                Text = "Frustration"
            };
            graph.HorizontalAxis.ArrowLength = 15;
            graph.HorizontalAxis.ArrowWidth = 10;
            graph.VerticalAxis.ArrowLength = 15;
            graph.VerticalAxis.ArrowWidth = 10;
            using (Image<Rgba32> img = new Image<Rgba32>(700, 700))
            {
                img.Mutate(mutator => mutator.Fill(Rgba32.White));
                graph.Render(img);
                img.Save("test.png");
            }
        }
    }
}
