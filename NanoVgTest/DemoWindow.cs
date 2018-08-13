using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanoVGDotNet;

namespace NanoVgTest
{
    class DemoWindow : IWindow
    {
        public void Build(MainWindow win)
        {
        }

        public void Load(MainWindow win)
        {
        }

        public void Unload(MainWindow win)
        {
        }

        public void Tick(MainWindow win)
        {
        }

        public void Render(MainWindow win, NVGcontext vg)
        {
            NanoVG.nvgSave(vg);

            NanoVG.nvgStrokeWidth(vg, 1);
            NanoVG.nvgStrokePaint(vg, NanoVG.nvgLinearGradient(vg, 10, 10, 50, 30, NanoVG.nvgRGBA(255, 255, 255, 255), NanoVG.nvgRGBA(0, 255, 0, 255)));

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRoundedRect(vg, 10, 10, 50, 30, 10);
            NanoVG.nvgStroke(vg);

            NanoVG.nvgRestore(vg);
        }
    }
}
