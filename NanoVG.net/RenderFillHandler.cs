namespace NanoVGDotNet
{
    public delegate void RenderFillHandler(object uptr, ref NvGpaint paint, ref NvGscissor scissor, float fringe, float[] bounds, NvGpath[] paths, int npaths);
}