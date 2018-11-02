namespace NanoVGDotNet
{
    public delegate void RenderTrianglesHandler(object uptr, ref NvGpaint paint, ref NvGscissor scissor,
        NvGvertex[] verts, int nverts);
}