using System;

namespace NanoVGDotNet
{
    public class NvGscissor
    {
        //[6];
        public float[] Xform;
        //[2];
        public float[] Extent;

        public NvGscissor()
        {
            Xform = new float[6];
            Extent = new float[2];
        }

        public NvGscissor Clone()
        {
            var newScissor = new NvGscissor();

            Array.Copy(Xform, newScissor.Xform, Xform.Length);
            Array.Copy(Extent, newScissor.Extent, Extent.Length);

            return newScissor;
        }
    }
}