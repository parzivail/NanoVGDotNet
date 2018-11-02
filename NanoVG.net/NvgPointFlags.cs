using System;

namespace NanoVGDotNet
{
    [Flags]
    public enum NvgPointFlags
    {
        Corner = 1,
        Left = 2,
        Bevel = 4,
        InnerBevel = 8
    }
}