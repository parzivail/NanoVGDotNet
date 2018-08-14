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

            NanoVG.nvgStrokeWidth(vg, 4);
            NanoVG.nvgStrokePaint(vg, NanoVG.nvgLinearGradient(vg, 10, 10, 50, 50, NanoVG.nvgRGBA(255, 255, 255, 255), NanoVG.nvgRGBA(0, 128, 0, 255)));
            NanoVG.nvgFillPaint(vg, NanoVG.nvgLinearGradient(vg, 10, 10, 50, 50, NanoVG.nvgRGBA(0, 128, 0, 255), NanoVG.nvgRGBA(255, 255, 255, 255)));

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, 10, 10, 50, 50);
            NanoVG.nvgFill(vg);
            NanoVG.nvgStroke(vg);

            NanoVG.nvgRestore(vg);
        }
    }
}
