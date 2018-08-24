using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using NanoVgTest.Shader;
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

        private ShaderProgram _shaderProgram;
        private uint _colorTexture;
        private uint _depthTexture;
        private uint _fboHandle;

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

            // Create Color Tex
            GL.GenTextures(1, out _colorTexture);
            GL.BindTexture(TextureTarget.Texture2D, _colorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            // GL.Ext.GenerateMipmap( GenerateMipmapTarget.Texture2D );

            // Create Depth Tex
            GL.GenTextures(1, out _depthTexture);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, Width, Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            // things go horribly wrong if DepthComponent's Bitcount does not match the main Framebuffer's Depth
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            // GL.Ext.GenerateMipmap( GenerateMipmapTarget.Texture2D );
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Create a FBO and attach the textures
            GL.Ext.GenFramebuffers(1, out _fboHandle);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, _fboHandle);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, _colorTexture, 0);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, _depthTexture, 0);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

            _shaderProgram = new DefaultShaderProgram(File.ReadAllText("Resources/Shader/shader.frag"), File.ReadAllText("Resources/Shader/shader.vert"));
            _shaderProgram.InitProgram();

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

            GL.BindTexture(TextureTarget.Texture2D, _colorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, _depthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, Width, Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void UpdateHandler(object sender, FrameEventArgs e)
        {
            if (_shouldDie)
                Exit();

            // Grab the new keyboard state
            Keyboard = OpenTK.Input.Keyboard.GetState();
            Mouse = OpenTK.Input.Mouse.GetState();

            _window.Tick(this);

            //TargetRenderFrequency = Focused ? 0 : 15;

            Title = $"FPS: {Math.Round(RenderFrequency)} | RenderTime: {Math.Round(RenderTime * 1000)}ms";

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
                Console.WriteLine($"GL Error: {err}");
        }

        private void RenderHandler(object sender, FrameEventArgs e)
        {
            GL.ClearColor(Color.FromArgb(255, 255, 255));
            // Reset the view
            GL.Clear(ClearBufferMask.ColorBufferBit |
                     ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, _fboHandle); // disable rendering into the FBO
            {
                GL.ClearColor(Color.FromArgb(13, 13, 13));
                // Reset the view
                GL.Clear(ClearBufferMask.ColorBufferBit |
                         ClearBufferMask.DepthBufferBit |
                         ClearBufferMask.StencilBufferBit);

                NanoVG.nvgBeginFrame(Nvg, Width, Height, 1);
                _window.Render(this, Nvg);
                NanoVG.nvgEndFrame(Nvg);
            }
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // disable rendering into the FBO

            GL.Disable(EnableCap.Texture2D);
            GL.PushMatrix();
            {
                GL.Color3(Color.White);
                GL.BindTexture(TextureTarget.Texture2D, _colorTexture);
                var uniforms = new[]
                {
                    new Uniform("iChannel0")
                    {
                        Value = 0
                    },
                    new Uniform("iTime")
                    {
                        Value = (float)DateTime.Now.TimeOfDay.TotalSeconds
                    },
                    new Uniform("iResolution")
                    {
                        Value = new Vector2(Width, Height)
                    },
                    new Uniform("enabled")
                    {
                        Value = !Keyboard[Key.S]
                    },
                    new Uniform("grainAmplitude")
                    {
                        Value = 0f//0.04f
                    },
                    new Uniform("maskSize")
                    {
                        Value = 2f
                    },
                    new Uniform("scanlineSize")
                    {
                        Value = 2f
                    },
                    new Uniform("jitterChance")
                    {
                        Value = 0.2f
                    },
                    new Uniform("trackingLossChance")
                    {
                        Value = 0.05f
                    },
                    new Uniform("hue")
                    {
                        Value = 0f
                    },
                    new Uniform("saturation")
                    {
                        Value = 30f
                    },
                    new Uniform("brightness")
                    {
                        Value = 1f
                    },
                    new Uniform("ntscColorFreqScale")
                    {
                        Value = 1f
                    },
                    new Uniform("ntscLumaFreqScale")
                    {
                        Value = 1f
                    },
                    new Uniform("ntscGrayscaleFreqScale")
                    {
                        Value = 1f
                    },
                    new Uniform("impulseResponseSize")
                    {
                        Value = 29
                    }
                };
                _shaderProgram.Use(uniforms);

                GL.Begin(PrimitiveType.Quads);
                {
                    GL.TexCoord2(0f, 1f);
                    GL.Vertex2(-1.0f, 1.0f);
                    GL.TexCoord2(0.0f, 0.0f);
                    GL.Vertex2(-1.0f, -1.0f);
                    GL.TexCoord2(1.0f, 0.0f);
                    GL.Vertex2(1.0f, -1.0f);
                    GL.TexCoord2(1.0f, 1.0f);
                    GL.Vertex2(1.0f, 1.0f);
                }
                GL.End();

                GL.BindTexture(TextureTarget.Texture2D, 0);

                GL.UseProgram(0);
            }
            GL.PopMatrix();
            GL.Disable(EnableCap.Texture2D);

            // Swap the graphics buffer
            SwapBuffers();
        }
    }
}