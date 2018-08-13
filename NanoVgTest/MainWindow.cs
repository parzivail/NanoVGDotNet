using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using NanoVGDotNet;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace NanoVgTest
{
    public class MainWindow : GameWindow
    {
        private readonly IWindow _window;
        private bool _shouldDie;

        public NVGcontext Nvg = new NVGcontext();
        public KeyboardState Keyboard;
        public MouseState Mouse;

        public MainWindow(IWindow window) : base(800, 600, new GraphicsMode(new ColorFormat(32), 24, 8, 0))
        {
            _window = window;
            // Wire up window
            Load += LoadHandler;
            Closing += CloseHandler;
            Resize += ResizeHandler;
            UpdateFrame += UpdateHandler;
            RenderFrame += RenderHandler;

            _window.Build(this);
        }

        private void LoadHandler(object sender, EventArgs e)
        {
            // Set up caps
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.RescaleNormal);

            // Set up blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            // Set background color
            GL.ClearColor(Color.FromArgb(13, 13, 13));

            // Init keyboard to ensure first frame won't NPE
            Keyboard = OpenTK.Input.Keyboard.GetState();
            Mouse = OpenTK.Input.Mouse.GetState();

            GlNanoVG.nvgCreateGL(ref Nvg, (int)NvgCreateFlags.AntiAlias | (int)NvgCreateFlags.StencilStrokes);

            _window.Load(this);
        }

        private void CloseHandler(object sender, CancelEventArgs e)
        {
            _window.Unload(this);
        }

        public void Kill()
        {
            _shouldDie = true;
        }

        private void ResizeHandler(object sender, EventArgs e)
        {
            GL.Viewport(ClientRectangle);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }

        private void UpdateHandler(object sender, FrameEventArgs e)
        {
            if (_shouldDie)
                Exit();

            // Grab the new keyboard state
            Keyboard = OpenTK.Input.Keyboard.GetState();
            Mouse = OpenTK.Input.Mouse.GetState();

            _window.Tick(this);
        }

        private void RenderHandler(object sender, FrameEventArgs e)
        {
            // Reset the view
            GL.Clear(ClearBufferMask.ColorBufferBit |
                     ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);

            NanoVG.nvgBeginFrame(Nvg, Width, Height, 1);
            _window.Render(this, Nvg);
            NanoVG.nvgEndFrame(Nvg);

            // Swap the graphics buffer
            SwapBuffers();
        }
    }
}