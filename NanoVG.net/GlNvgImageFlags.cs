using System;

namespace NanoVGDotNet
{
    [Flags]
    public enum GlNvgImageFlags
    {
        // Do not delete GL texture handle.
        NoDelete = 65536
    }
}