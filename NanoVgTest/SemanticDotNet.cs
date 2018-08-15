using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanoVGDotNet;

namespace NanoVgTest
{
    class SemanticDotNet
    {
        public class SStyle
        {
            public string FontSans { get; set; } = "sans";
            public string FontIcon { get; set; } = "icon";
            public float FontSize { get; set; } = 17;
            public float IconPadding { get; set; } = 5;

            public NVGcolor FontDefaultColor { get; set; } = NanoVG.nvgRGBA(0, 0, 0, 153);
            public NVGcolor FontHoverColor { get; set; } = NanoVG.nvgRGBA(0, 0, 0, 204);
            public NVGcolor FontFocusColor { get; set; } = NanoVG.nvgRGBA(0, 0, 0, 242);

            public float ButtonHeight { get; set; } = 36;

            public float BorderRadius { get; set; } = 4;

            public NVGcolor ButtonBackgroundDefaultColor { get; set; } = NanoVG.nvgRGBA(224, 225, 226, 255);
            public NVGcolor ButtonBackgroundHoverColor { get; set; } = NanoVG.nvgRGBA(202, 203, 205, 255);
            public NVGcolor ButtonBackgroundFocusColor { get; set; } = NanoVG.nvgRGBA(186, 187, 188, 255);
        }

        public enum SWidgetState
        {
            Default,
            Hover,
            Focus
        }

        public static readonly SStyle StyleDefault = new SStyle();
        public static SStyle Style { get; set; } = StyleDefault;

        public static void SdnTextButton(NVGcontext ctx, float x, float y, float w, float h, string text, SWidgetState state)
        {
            Button(ctx, x, y, w, h, Style.FontSans, text, state);
        }

        public static void SdnIconButton(NVGcontext ctx, float x, float y, float w, float h, string icon, SWidgetState state)
        {
            Button(ctx, x, y, w, h, Style.FontIcon, icon, state);
        }

        private static void Button(NVGcontext ctx, float x, float y, float w, float h, string font, string text, SWidgetState state)
        {
            NanoVG.nvgSave(ctx);

            SetButtonBgColor(ctx, state);

            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgRoundedRect(ctx, x, y, w, h, Style.BorderRadius);
            NanoVG.nvgFill(ctx);

            SetFontStyle(ctx, state);

            NanoVG.nvgFontFace(ctx, font);
            var b = new float[4];
            NanoVG.nvgTextBounds(ctx, 0, 0, text, b);
            var fw = b[2] - b[0];
            var fh = b[3] - b[1];

            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgText(ctx, Round(x + (w - fw) / 2), Round(y + (h - fh) / 2), text);
            NanoVG.nvgFill(ctx);

            NanoVG.nvgRestore(ctx);
        }

        public static void SdnIconTextButton(NVGcontext ctx, float x, float y, float w, float h, string icon, string text, SWidgetState state)
        {
            NanoVG.nvgSave(ctx);

            SetButtonBgColor(ctx, state);

            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgRoundedRect(ctx, x, y, w, h, Style.BorderRadius);
            NanoVG.nvgFill(ctx);

            SetFontStyle(ctx, state);

            NanoVG.nvgFontFace(ctx, Style.FontIcon);
            var ib = new float[4];
            NanoVG.nvgTextBounds(ctx, 0, 0, icon, ib);
            var ifw = ib[2] - ib[0];

            NanoVG.nvgFontFace(ctx, Style.FontSans);
            var sb = new float[4];
            NanoVG.nvgTextBounds(ctx, 0, 0, text, sb);
            var sfw = sb[2] - sb[0];
            var sfh = sb[3] - sb[1];

            var fw = ifw + sfw + Style.IconPadding;

            NanoVG.nvgFontFace(ctx, Style.FontIcon);
            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgText(ctx, Round(x + (w - fw) / 2), Round(y + (h - sfh) / 2), icon);
            NanoVG.nvgFill(ctx);

            NanoVG.nvgFontFace(ctx, Style.FontSans);
            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgText(ctx, Round(x + ifw + Style.IconPadding + (w - fw) / 2), Round(y + (h - sfh) / 2), text);
            NanoVG.nvgFill(ctx);

            NanoVG.nvgRestore(ctx);
        }

        private static void SetFontStyle(NVGcontext ctx, SWidgetState state)
        {
            NanoVG.nvgFontSize(ctx, Style.FontSize);
            NanoVG.nvgTextAlign(ctx, (int)NvgAlign.Top | (int)NvgAlign.Left);
            SetFontColor(ctx, state);
        }

        private static float Round(float x)
        {
            return (float)Math.Round(x);
        }

        private static void SetButtonBgColor(NVGcontext ctx, SWidgetState state)
        {
            switch (state)
            {
                case SWidgetState.Default:
                    NanoVG.nvgFillColor(ctx, Style.ButtonBackgroundDefaultColor);
                    break;
                case SWidgetState.Hover:
                    NanoVG.nvgFillColor(ctx, Style.ButtonBackgroundHoverColor);
                    break;
                case SWidgetState.Focus:
                    NanoVG.nvgFillColor(ctx, Style.ButtonBackgroundFocusColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void SetFontColor(NVGcontext ctx, SWidgetState state)
        {
            switch (state)
            {
                case SWidgetState.Default:
                    NanoVG.nvgFillColor(ctx, Style.FontDefaultColor);
                    break;
                case SWidgetState.Hover:
                    NanoVG.nvgFillColor(ctx, Style.FontHoverColor);
                    break;
                case SWidgetState.Focus:
                    NanoVG.nvgFillColor(ctx, Style.FontFocusColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
