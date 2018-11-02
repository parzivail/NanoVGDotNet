#define NANOVG_GL3_IMPLEMENTATION
#define NANOVG_GL_USE_STATE_FILTER

#if NANOVG_GL2_IMPLEMENTATION
#define NANOVG_GL2
#define NANOVG_GL_IMPLEMENTATION
#elif NANOVG_GL3_IMPLEMENTATION
#define NANOVG_GL3
#define NANOVG_GL_IMPLEMENTATION
//#define NANOVG_GL_USE_UNIFORMBUFFER
#endif

using OpenTK.Graphics.OpenGL;

namespace NanoVGDotNet
{
    public class GlnvGcontext
    {
        public GlnvGshader Shader;
        public GlnvGtexture[] Textures;
        // [2]
        public float[] View;
        public int Ntextures;
        public int Ctextures;
        public int TextureId;
        public uint VertBuf;
#if NANOVG_GL3
        public uint VertArr;
#endif
#if NANOVG_GL_USE_UNIFORMBUFFER
		public uint fragBuf;
#endif
        public int FragSize;
        public int Flags;

        // Per frame buffers
        public GlnvGcall[] Calls;
        public int Ccalls;
        public int Ncalls;
        public GlnvGpath[] Paths;
        public int Cpaths;
        public int Npaths;
        public NvGvertex[] Verts;
        public int Cverts;
        public int Nverts;
        public GlnvGfragUniforms[] Uniforms;
        public int Cuniforms;
        public int Nuniforms;

        // cached state
#if NANOVG_GL_USE_STATE_FILTER
        public uint BoundTexture;
        public uint StencilMask;
        public StencilFunction StencilFunc;
        public int StencilFuncRef;
        public uint StencilFuncMask;
#endif

        public GlnvGcontext()
        {
            View = new float[2];
        }
    }
}