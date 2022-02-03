using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace SharpCanvas
{
    public partial class PlotWindow : GameWindow
    {
        private Stack<(int, int)> _segmentations = new Stack<(int, int)>();
        private Vector3[] _pixels;
        HittableList _world = SceneGenerator.GenerateBouncingMarbles();
        Camera _camera;

        private float _timeSpent = 0.0f;
        private int _segmentsDone = 0;
        private int _samplesDone = SamplesPerPixel;

        const int SegmentationSize = 64;
        const int SamplesPerPixel = 1;
        const int MaxDepth = 50;

        private Color4 RayColor(Ray ray, ref HittableList world, int depth)
        {
            if (depth <= 0)
            {
                return new Color4(0f, 0f, 0f, 1f);
            }

            HitRecord record = new HitRecord();

            if(world.Hit(ray, 0.001f, float.PositiveInfinity, ref record))
            {
                Ray scattered = new Ray(Vector3.Zero, Vector3.Zero);
                Color4 attenuation = new Color4();

                var scatter = record.Material.Scatter(ray, record, ref attenuation, ref scattered);

                if(scatter)
                {
                    var color = RayColor(scattered, ref world, depth - 1);
                    return new Color4(attenuation.R * color.R, attenuation.G * color.G, attenuation.B * color.B, 1f);
                }

                return Color4.Black;
            }

            var unit_direction = ray.Direction.Normalized();
            var t = 0.5f * (unit_direction.Y  + 1.0f);
            return new Color4((1f - t) + (t * 0.5f), (1f - t) + (t * 0.7f), (1f - t) + (t * 1f), 1f);
        }

        private void Begin()
        {        
            var lookFrom = new Vector3(13f, 2f, 3f);
            var lookAt = new Vector3(0f, 0f, 0f);
            var distToFocus = 10f;
            var apreture = 0.1f;

            _camera = new Camera(lookFrom, lookAt, new Vector3(0f, 1f, 0f), 20, AspectRatio, apreture, distToFocus, 0, 1f);

            _pixels = new Vector3[Width * Height];

            var tracerThread = new Thread(() => {
                Console.WriteLine($"Creating {(Width * Height)/(SegmentationSize * SegmentationSize)} segmentation for rendering {SamplesPerPixel} samples.");
                while(true)
                {
                    for (int i = 0; i < Width * Height; i += SegmentationSize * SegmentationSize)
                    {
                        _segmentations.Push((i, i + SegmentationSize * SegmentationSize));
                    }
                    Cast(ref _renderer.Canvas);
                    _samplesDone += SamplesPerPixel;
                }
            });

            tracerThread.Start();
        }

        private void PrintColor(int j, int i, Vector3 color)
        {
            var index = _renderer.Canvas.ConvertIndex(j, i);

            if(index < 0 || index > Width * Height)
                return;

            var r = color.X + _pixels[index].X;
            var g = color.Y + _pixels[index].Y;
            var b = color.Z + _pixels[index].Z;

            var scale = 1.0f / _samplesDone;
            r = MathF.Sqrt(scale * r);
            g = MathF.Sqrt(scale * g);
            b = MathF.Sqrt(scale * b);

            _pixels[index] += color;
            _renderer.Canvas.Pixels[index] = new Color4(r, g, b, 1f);
        }

        private void Cast(ref Canvas screen)
        {
            var watch = new Stopwatch();
            watch.Start();

            var threadCount = _segmentations.Count;

            while(_segmentations.TryPop(out var segment))
            {
                ThreadPool.QueueUserWorkItem((callback) => {
                    var localSegment = segment;
                    SampleSegment(localSegment);
                    _segmentsDone++;
                });
            }
            
            while(threadCount > _segmentsDone);
            
            watch.Stop();

            _timeSpent += watch.ElapsedMilliseconds;

            Console.WriteLine($"Rendered {_samplesDone} samples after {(watch.ElapsedMilliseconds) / 1000f}s AVG: {(_timeSpent / (_samplesDone / SamplesPerPixel)) / 1000f}s TOT: {_timeSpent / 1000f}s APT: {(((_timeSpent / (_samplesDone / SamplesPerPixel)) / (double)(Width * Height)))}ms AST: {((((_timeSpent / (_samplesDone / SamplesPerPixel)) / (double)(Width * Height)))) / (double)SamplesPerPixel}ms.");
            _segmentsDone = 0;
        }

        private void SampleSegment((int, int) segment)
        {
            for (int i = segment.Item2; i >= segment.Item1; i--)
            {
                var x = i % Width;
                var y = i / Width;

                var r = 0.0f;
                var g = 0.0f;
                var b = 0.0f;

                for (int s = 0; s < SamplesPerPixel; s++)
                {
                    var u = (float)x / (Width - 1);
                    var v = (float)y / (Height - 1);

                    Ray ray = _camera.GetRay(u, v);
                                
                    var rayColor = RayColor(ray, ref _world, MaxDepth);
                    
                    r += rayColor.R;
                    g += rayColor.G;
                    b += rayColor.B;
                }

                PrintColor(x, y, new Vector3(r, g, b));
            }
        }

        private void Sample()
        {
            for (int j = Height - 1; j >= 0; --j)
            {
                for (int i = 0; i < Width; ++i)
                {
                    var r = 0.0f;
                    var g = 0.0f;
                    var b = 0.0f;

                    for (int s = 0; s < SamplesPerPixel; s++)
                    {
                        var u = (float)i / (Width - 1);
                        var v = (float)j / (Height - 1);

                        Ray ray = _camera.GetRay(u, v);
                        
                        var rayColor = RayColor(ray, ref _world, MaxDepth);
                        
                        r += rayColor.R;
                        g += rayColor.G;
                        b += rayColor.B;

                    }
                    PrintColor(i, j, new Vector3(r, g, b));
                }
            }
        }

        private void Input(float delta)
        {

        }

        private void Update(float delta)
        {
        }

        private void Draw(ref Canvas screen)
        {

        }
    }
}