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
            var rIcons = win.Nvg.CreateFont("icon",
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}{MaterialDesignIcons.FontIconFileName}");
            if (rIcons == -1)
                Console.WriteLine("Unable to load icons");

            var rSans = win.Nvg.CreateFont("sans",
                $"Resources{Path.DirectorySeparatorChar}Fonts{Path.DirectorySeparatorChar}neuehaasgrotesk.ttf");
            if (rSans == -1)
                Console.WriteLine("Unable to load sans");

            var rVcr = win.Nvg.CreateFont("vcr",
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
            vg.Save();

            //            SemanticDotNet.SdnTextButton(vg, 30, 30, 75, SemanticDotNet.Style.ButtonHeight, "Button", connection: SemanticDotNet.SConnectedSide.Right);
            //            SemanticDotNet.SdnIconTextButton(vg, 105, 30, 85, SemanticDotNet.Style.ButtonHeight, MaterialDesignIcons.Qrcode, "Icon", connection: SemanticDotNet.SConnectedSide.Left);
            //
            //            SemanticDotNet.SdnSplitIconTextButton(vg, 30, 75, 36, 75, SemanticDotNet.Style.ButtonHeight, MaterialDesignIcons.BrightnessAuto, "Auto");
            //
            //            SemanticDotNet.SdnCheckbox(vg, 30, 120, 17, true, "Checkbox");

            vg.FillColor(NanoVg.Rgba(255, 255, 255, 255));
            vg.FontFace("sans");
            vg.FontSize(48);
            vg.TextAlign(NvgAlign.Top | NvgAlign.Left);

            vg.Text(50, 50, "commercial towing vehicle 'The Nostromo'");
            vg.Text(150, 140, "crew:");
            vg.Text(290, 140, "seven");
            vg.Text(150, 190, "cargo:");
            vg.Text(290, 190, "refinery processing");
            vg.Text(290, 230, "20,000,000 tons of mineral ore");
            vg.Text(150, 280, "course:");
            vg.Text(290, 280, "returning to earth");

            vg.Save();
            vg.Translate(350, 450);

            vg.FillColor(NanoVg.Rgba(255, 0, 0, 255));
            vg.StrokeWidth(4);
            vg.BeginPath();
            vg.Rect(0, 0, 80, 80);
            vg.Fill();

            vg.FillColor(NanoVg.Rgba(0, 255, 128, 255));
            vg.StrokeWidth(4);
            vg.BeginPath();
            vg.Rect(50, 30, 80, 80);
            vg.Fill();

            vg.FillPaint(NanoVg.ImagePattern(vg, 150, 30, 220, 330, 0, _imgCat, 1));
            vg.BeginPath();
            vg.Rect(150, 30, 220, 330);
            vg.Fill();
            
            vg.FillPaint(NanoVg.ImagePattern(vg, 380, 30, 1000, 420, 0, _imgPlanes, 1));
            vg.BeginPath();
            vg.Rect(380, 30, 1000, 420);
            vg.Fill();

            vg.Restore();

            vg.Restore();
        }
    }
}
