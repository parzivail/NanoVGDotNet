namespace NanoVGDotNet
{
    public class GLNVGshader
    {
        public int prog;
        public int frag;
        public int vert;
        //[GLNVG_MAX_LOCS];
        public int[] loc;

        public GLNVGshader()
        {
            loc = new int[(int)GlNvgUniformLoc.MaxLocs];
        }
    }
}