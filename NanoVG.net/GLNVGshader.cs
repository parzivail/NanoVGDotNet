namespace NanoVGDotNet
{
    public class GlnvGshader
    {
        public int Prog;
        public int Frag;
        public int Vert;
        //[GLNVG_MAX_LOCS];
        public int[] Loc;

        public GlnvGshader()
        {
            Loc = new int[(int)GlNvgUniformLoc.MaxLocs];
        }
    }
}