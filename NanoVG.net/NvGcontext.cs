using FontStashDotNet;

namespace NanoVGDotNet
{
    public class NvGcontext
    {
        public NvGparams Params;
        public float[] Commands;
        public int Ccommands;
        public int Ncommands;
        public float Commandx, Commandy;
        //[NVG_MAX_STATES];
        public NvGstate[] States;
        public int Nstates;
        public NvGpathCache Cache;
        public float TessTol;
        public float DistTol;
        public float FringeWidth;
        public float DevicePxRatio;
        public FONScontext Fs;
        //[NVG_MAX_FONTIMAGES];
        public int[] FontImages;
        public int FontImageIdx;
        public int DrawCallCount;
        public int FillTriCount;
        public int StrokeTriCount;
        public int TextTriCount;

        public NvGcontext()
        {
            States = new NvGstate[NanoVg.NvgMaxStates];
            for (var cont = 0; cont < States.Length; cont++)
                States[cont] = new NvGstate();
            FontImages = new int[NanoVg.NvgMaxFontimages];
        }
    }
}