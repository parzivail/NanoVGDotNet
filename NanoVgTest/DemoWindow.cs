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
        private int _imgCat;
        private int _imgPlanes;

        public void Build(MainWindow win)
        {
        }

        public void Load(MainWindow win)
        {
            var rIcons = NanoVg.CreateFont(win.Nvg, "icon",
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}{MaterialDesignIcons.FontIconFileName}");
            if (rIcons == -1)
                Console.WriteLine("Unable to load icons");

            var rSans = NanoVg.CreateFont(win.Nvg, "sans",
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}neuehaasgrotesk.ttf");
            if (rSans == -1)
                Console.WriteLine("Unable to load sans");

            var rVcr = NanoVg.CreateFont(win.Nvg, "vcr",
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}vcrmono.ttf");
            if (rVcr == -1)
                Console.WriteLine("Unable to load vcr");

            _imgCat = NanoVg.CreateImage(ref win.Nvg, "Resources/Images/cat.png", 0);
            _imgPlanes = NanoVg.CreateImage(ref win.Nvg, "Resources/Images/planes.png", 0);
        }

        public void Unload(MainWindow win)
        {
        }

        public void Tick(MainWindow win)
        {
        }

        public void Render(MainWindow win, NvgContext vg)
        {
            NanoVg.Save(vg);

            //            SemanticDotNet.SdnTextButton(vg, 30, 30, 75, SemanticDotNet.Style.ButtonHeight, "Button", connection: SemanticDotNet.SConnectedSide.Right);
            //            SemanticDotNet.SdnIconTextButton(vg, 105, 30, 85, SemanticDotNet.Style.ButtonHeight, MaterialDesignIcons.Qrcode, "Icon", connection: SemanticDotNet.SConnectedSide.Left);
            //
            //            SemanticDotNet.SdnSplitIconTextButton(vg, 30, 75, 36, 75, SemanticDotNet.Style.ButtonHeight, MaterialDesignIcons.BrightnessAuto, "Auto");
            //
            //            SemanticDotNet.SdnCheckbox(vg, 30, 120, 17, true, "Checkbox");

            NanoVg.FillColor(vg, NanoVg.Rgba(255, 255, 255, 255));
            NanoVg.FontFace(vg, "sans");
            NanoVg.FontSize(vg, 48);
            NanoVg.TextAlign(vg, (int)NvgAlign.Top | (int)NvgAlign.Left);

            NanoVg.Text(vg, 50, 50, "commercial towing vehicle 'The Nostromo'");
            NanoVg.Text(vg, 150, 140, "crew:");
            NanoVg.Text(vg, 290, 140, "seven");
            NanoVg.Text(vg, 150, 190, "cargo:");
            NanoVg.Text(vg, 290, 190, "refinery processing");
            NanoVg.Text(vg, 290, 230, "20,000,000 tons of mineral ore");
            NanoVg.Text(vg, 150, 280, "course:");
            NanoVg.Text(vg, 290, 280, "returning to earth");

            NanoVg.Save(vg);
            NanoVg.Translate(vg, 350, 450);

            NanoVg.FillColor(vg, NanoVg.Rgba(255, 0, 0, 255));
            NanoVg.StrokeWidth(vg, 4);
            NanoVg.BeginPath(vg);
            NanoVg.Rect(vg, 0, 0, 80, 80);
            NanoVg.Fill(vg);

            NanoVg.FillColor(vg, NanoVg.Rgba(0, 255, 128, 255));
            NanoVg.StrokeWidth(vg, 4);
            NanoVg.BeginPath(vg);
            NanoVg.Rect(vg, 50, 30, 80, 80);
            NanoVg.Fill(vg);

            NanoVg.FillPaint(vg, NanoVg.ImagePattern(vg, 150, 30, 220, 330, 0, _imgCat, 1));
            NanoVg.BeginPath(vg);
            NanoVg.Rect(vg, 150, 30, 220, 330);
            NanoVg.Fill(vg);
            
            NanoVg.FillPaint(vg, NanoVg.ImagePattern(vg, 380, 30, 1000, 420, 0, _imgPlanes, 1));
            NanoVg.BeginPath(vg);
            NanoVg.Rect(vg, 380, 30, 1000, 420);
            NanoVg.Fill(vg);

            NanoVg.Restore(vg);

            NanoVg.Restore(vg);
        }
    }
}
