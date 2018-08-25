using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanoVGDotNet;
using OpenTK.Input;

namespace NanoVgTest
{
    class DemoWindow : IWindow
    {
        public void Build(MainWindow win)
        {
        }

        public void Load(MainWindow win)
        {
            var rIcons = NanoVG.nvgCreateFont(win.Nvg, "icon",
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}{MaterialDesignIcons.FontIconFileName}");
            if (rIcons == -1)
                Console.WriteLine("Unable to load icons");

            var rSans = NanoVG.nvgCreateFont(win.Nvg, "sans",
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}neuehaasgrotesk.ttf");
            if (rSans == -1)
                Console.WriteLine("Unable to load sans");

            var rVcr = NanoVG.nvgCreateFont(win.Nvg, "vcr",
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}vcrmono.ttf");
            if (rVcr == -1)
                Console.WriteLine("Unable to load vcr");
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

            //            SemanticDotNet.SdnTextButton(vg, 30, 30, 75, SemanticDotNet.Style.ButtonHeight, "Button", connection: SemanticDotNet.SConnectedSide.Right);
            //            SemanticDotNet.SdnIconTextButton(vg, 105, 30, 85, SemanticDotNet.Style.ButtonHeight, MaterialDesignIcons.Qrcode, "Icon", connection: SemanticDotNet.SConnectedSide.Left);
            //
            //            SemanticDotNet.SdnSplitIconTextButton(vg, 30, 75, 36, 75, SemanticDotNet.Style.ButtonHeight, MaterialDesignIcons.BrightnessAuto, "Auto");
            //
            //            SemanticDotNet.SdnCheckbox(vg, 30, 120, 17, true, "Checkbox");

            NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 255, 255, 255));
            NanoVG.nvgFontFace(vg, "sans");
            NanoVG.nvgFontSize(vg, 48);
            NanoVG.nvgTextAlign(vg, (int)NvgAlign.Top | (int)NvgAlign.Left);

            NanoVG.nvgText(vg, 50, 50, "commercial towing vehicle 'The Nostromo'");
            NanoVG.nvgText(vg, 150, 140, "crew:");
            NanoVG.nvgText(vg, 290, 140, "seven");
            NanoVG.nvgText(vg, 150, 190, "cargo:");
            NanoVG.nvgText(vg, 290, 190, "refinery processing");
            NanoVG.nvgText(vg, 290, 230, "20,000,000 tons of mineral ore");
            NanoVG.nvgText(vg, 150, 280, "course:");
            NanoVG.nvgText(vg, 290, 280, "returning to earth");

            NanoVG.nvgSave(vg);
            NanoVG.nvgTranslate(vg, 350, 450);

            NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(255, 0, 0, 255));
            NanoVG.nvgStrokeWidth(vg, 4);
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, 0, 0, 80, 80);
            NanoVG.nvgFill(vg);

            NanoVG.nvgFillColor(vg, NanoVG.nvgRGBA(0, 255, 128, 255));
            NanoVG.nvgStrokeWidth(vg, 4);
            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, 50, 30, 80, 80);
            NanoVG.nvgFill(vg);

            NanoVG.nvgRestore(vg);

            NanoVG.nvgRestore(vg);
        }
    }
}
