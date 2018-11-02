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
    public class GLNVGcontext
    {
        public GLNVGshader shader;
        public GLNVGtexture[] textures;
        // [2]
        public float[] view;
        public int ntextures;
        public int ctextures;
        public int textureId;
        public uint vertBuf;
#if NANOVG_GL3
        public uint vertArr;
#endif
#if NANOVG_GL_USE_UNIFORMBUFFER
		public uint fragBuf;
#endif
        public int fragSize;
        public int flags;

        // Per frame buffers
        public GLNVGcall[] calls;
        public int ccalls;
        public int ncalls;
        public GLNVGpath[] paths;
        public int cpaths;
        public int npaths;
        public NVGvertex[] verts;
        public int cverts;
        public int nverts;
        public GLNVGfragUniforms[] uniforms;
        public int cuniforms;
        public int nuniforms;

        // cached state
#if NANOVG_GL_USE_STATE_FILTER
        public uint boundTexture;
        public uint stencilMask;
        public StencilFunction stencilFunc;
        public int stencilFuncRef;
        public uint stencilFuncMask;
#endif

        public GLNVGcontext()
        {
            view = new float[2];
        }
    }
}