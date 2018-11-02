namespace NanoVGDotNet
{
    public delegate void RenderStrokeHandler(object uptr, ref NvGpaint paint, ref NvGscissor scissor, float fringe, float strokeWidth, NvGpath[] paths, int npaths);
}