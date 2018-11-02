namespace NanoVGDotNet
{
    public struct NvGparams
    {
        public object UserPtr;
        public int EdgeAntiAlias;
        public RenderCreateHandler RenderCreate;

        public RenderCreateTextureHandler RenderCreateTexture;
        public RenderCreateTextureHandler2 RenderCreateTexture2;
        public RenderViewportHandler RenderViewport;
        public RenderFlushHandler RenderFlush;
        public RenderFillHandler RenderFill;
        public RenderStrokeHandler RenderStroke;
        public RenderTrianglesHandler RenderTriangles;
        public RenderUpdateTextureHandler RenderUpdateTexture;
        public RenderGetTextureSizeHandler RenderGetTextureSize;
        public RenderDeleteTexture RenderDeleteTexture;
        public RenderCancel RenderCancel;
        public RenderDelete RenderDelete;
    }
}