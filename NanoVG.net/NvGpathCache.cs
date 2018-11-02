namespace NanoVGDotNet
{
    public class NvGpathCache
    {
        public NvGpoint[] Points;
        public int Npoints;
        public int Cpoints;
        public NvGpath[] Paths;
        public int Npaths;
        public int Cpaths;
        public NvGvertex[] Verts;
        public int Nverts;
        public int Cverts;
        //[4];
        public float[] Bounds;

        public NvGpathCache()
        {
            Bounds = new float[4];
        }
    }
}