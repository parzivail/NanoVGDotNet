using NanoVGDotNet;

namespace NanoVgTest
{
    public interface IWindow
    {
        void Build(MainWindow win);
        void Load(MainWindow win);
        void Unload(MainWindow win);
        void Tick(MainWindow win);
        void Render(MainWindow win, NVGcontext vg);
    }
}