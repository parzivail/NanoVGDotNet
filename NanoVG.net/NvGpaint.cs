using System;

namespace NanoVGDotNet
{
    public class NvGpaint
    {
        //[6];
        public float[] Xform;
        //[2];
        public float[] Extent;
        public float Radius;
        public float Feather;
        public NvGcolor InnerColor;
        public NvGcolor OuterColor;
        public int Image;

        public NvGpaint()
        {
            Xform = new float[6];
            Extent = new float[2];
        }

        public NvGpaint Clone()
        {
            var newPaint = new NvGpaint();

            Array.Copy(Xform, newPaint.Xform, Xform.Length);
            Array.Copy(Extent, newPaint.Extent, Extent.Length);
            newPaint.Radius = Radius;
            newPaint.Feather = Feather;
            newPaint.InnerColor = InnerColor;
            newPaint.OuterColor = OuterColor;
            newPaint.Image = Image;

            return newPaint;
        }
    }
}