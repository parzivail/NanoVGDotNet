using System;

namespace NanoVGDotNet
{
    public class NvGstate
    {
        public NvGcompositeOperationState CompositeOperation;
        public NvGpaint Fill;
        public NvGpaint Stroke;
        public float StrokeWidth;
        public float MiterLimit;
        public int LineJoin;
        public int LineCap;
        public float Alpha;
        //[6];
        public float[] Xform;
        public NvGscissor Scissor;
        public float FontSize;
        public float LetterSpacing;
        public float LineHeight;
        public float FontBlur;
        public int TextAlign;
        public int FontId;

        public NvGstate()
        {
            Xform = new float[6];
            Scissor = new NvGscissor();
            Fill = new NvGpaint();
            Stroke = new NvGpaint();
        }

        public NvGstate Clone()
        {
            var newState = new NvGstate();
            newState.CompositeOperation = CompositeOperation;
            newState.Fill = Fill.Clone();
            newState.Stroke = Stroke.Clone();
            newState.StrokeWidth = StrokeWidth;
            newState.MiterLimit = MiterLimit;
            newState.LineJoin = LineJoin;
            newState.LineCap = LineCap;
            newState.Alpha = Alpha;

            Array.Copy(Xform, newState.Xform, Xform.Length);

            newState.Scissor = Scissor.Clone();
            newState.FontSize = FontSize;
            newState.LetterSpacing = LetterSpacing;
            newState.LineHeight = LineHeight;
            newState.FontBlur = FontBlur;
            newState.TextAlign = TextAlign;
            newState.FontId = FontId;

            return newState;
        }
    }
}