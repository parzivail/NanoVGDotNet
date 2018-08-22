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
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}latosemi.ttf");
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
            NanoVG.nvgStrokeColor(vg, NanoVG.nvgRGBA(0, 255, 0, 255));
            NanoVG.nvgStrokeWidth(vg, 2);
            NanoVG.nvgFontFace(vg, "vcr");
            NanoVG.nvgFontSize(vg, 48);
            NanoVG.nvgTextAlign(vg, (int)NvgAlign.Top | (int)NvgAlign.Left);
            
            NanoVG.nvgText(vg, 50, 50, "Commercial Mining Ship 'The Nostromo'");

            NanoVG.nvgSave(vg);
            NanoVG.nvgTranslate(vg, 250, 250);
            NanoVG.nvgRotate(vg, (float) (DateTime.Now.TimeOfDay.TotalSeconds % 10 / 10f * 2 * Math.PI));

            NanoVG.nvgBeginPath(vg);
            NanoVG.nvgRect(vg, -20, -20, 40, 40);
            NanoVG.nvgStroke(vg);

            NanoVG.nvgRestore(vg);

            NanoVG.nvgRestore(vg);
        }
    }
}
