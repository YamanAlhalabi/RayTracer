using System;
using System.Threading;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SharpCanvas
{
    public partial class PlotWindow : GameWindow
    {
        public float AspectRatio = 16f / 9f;
        public int Width, Height;
        private CanvasRenderer _renderer;
        public PlotWindow() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            AspectRatio = 1f / 1f;
            Width = 512;
            Height = (int)(Width / AspectRatio);

            Size = new Vector2i(Width, Height);

            new Debug();

        }
        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }
        protected override void OnLoad()
        {
            _renderer = new CanvasRenderer();
            _renderer.Canvas = new Canvas();
            _renderer.Canvas.InitializeCanvas(Width, Height);
            _renderer.Create();
            Begin();
            base.OnLoad();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            Input((float)args.Time);
            Update((float)args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(1f, 0f, 1f, 1f);

            Draw(ref _renderer.Canvas);
            _renderer.Render();
            SwapBuffers();
        }
    }
}