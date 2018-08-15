using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

            public NVGcolor PrimaryDefaultColor { get; set; } = NanoVG.nvgRGBA(224, 225, 226, 255);
            public NVGcolor PrimaryHoverColor { get; set; } = NanoVG.nvgRGBA(202, 203, 205, 255);
            public NVGcolor PrimaryFocusColor { get; set; } = NanoVG.nvgRGBA(186, 187, 188, 255);
        }

        public enum SWidgetState
        {
            Default,
            Hover,
            Focus
        }

        [Flags]
        public enum SConnectedSide
        {
            None = 0,
            Top = 1,
            Bottom = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3
        }

        public static readonly SStyle StyleDefault = new SStyle();
        public static readonly SStyle StyleConnectedBtn = new SStyle
        {
            PrimaryDefaultColor = NanoVG.nvgRGBA(214, 215, 216, 255),
            PrimaryHoverColor = NanoVG.nvgRGBA(192, 193, 195, 255),
            PrimaryFocusColor = NanoVG.nvgRGBA(176, 177, 178, 255)
        };

        public static SStyle Style { get; set; } = StyleDefault;

        public static void SdnTextButton(NVGcontext ctx, float x, float y, float w, float h, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            Button(ctx, x, y, w, h, Style.FontSans, text, state, connection);
        }

        public static void SdnIconButton(NVGcontext ctx, float x, float y, float w, float h, string icon, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            Button(ctx, x, y, w, h, Style.FontIcon, icon, state);
        }

        public static void SdnCheckbox(NVGcontext ctx, float x, float y, float size, bool @checked, string label,
            SWidgetState state = SWidgetState.Default)
        {
            NanoVG.nvgSave(ctx);

            SetPrimaryStrokeColor(ctx, state);

            NanoVG.nvgStrokeWidth(ctx, 2);

            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgRoundedRect(ctx, x, y, size, size, 4);
            NanoVG.nvgStroke(ctx);

            SetFontStyle(ctx, state);

            NanoVG.nvgFontFace(ctx, Style.FontSans);
            var b = new float[4];
            NanoVG.nvgTextBounds(ctx, 0, 0, label, b);
            var fw = b[2] - b[0];
            var fh = b[3] - b[1];

            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgText(ctx, Round(x + size + 10), Round(y + (size - fh) / 2), label);
            NanoVG.nvgFill(ctx);

            NanoVG.nvgRestore(ctx);
        }

        private static void Button(NVGcontext ctx, float x, float y, float w, float h, string font, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            NanoVG.nvgSave(ctx);

            SetPrimaryFillColor(ctx, state);
            DrawConnectedRect(ctx, x, y, w, h, connection);

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

        public static void SdnIconTextButton(NVGcontext ctx, float x, float y, float w, float h, string icon, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            NanoVG.nvgSave(ctx);

            SetPrimaryFillColor(ctx, state);
            DrawConnectedRect(ctx, x, y, w, h, connection);

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

        public static void SdnSplitIconTextButton(NVGcontext ctx, float x, float y, float splitWidth, float w, float h, string icon, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            NanoVG.nvgSave(ctx);

            WithStyle(StyleConnectedBtn, () =>
            {
                SetPrimaryFillColor(ctx, state);
                DrawConnectedRect(ctx, x, y, splitWidth, h, connection | SConnectedSide.Right);
            });

            SetPrimaryFillColor(ctx, state);
            DrawConnectedRect(ctx, x + splitWidth, y, w, h, connection | SConnectedSide.Left);

            SetFontStyle(ctx, state);

            NanoVG.nvgFontFace(ctx, Style.FontIcon);
            var ib = new float[4];
            NanoVG.nvgTextBounds(ctx, 0, 0, icon, ib);
            var ifw = ib[2] - ib[0];
            var ifh = ib[3] - ib[1];

            NanoVG.nvgFontFace(ctx, Style.FontSans);
            var sb = new float[4];
            NanoVG.nvgTextBounds(ctx, 0, 0, text, sb);
            var sfw = sb[2] - sb[0];
            var sfh = sb[3] - sb[1];

            NanoVG.nvgFontFace(ctx, Style.FontIcon);
            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgText(ctx, Round(x + (splitWidth - ifw) / 2), Round(y + (h - ifh) / 2), icon);
            NanoVG.nvgFill(ctx);

            NanoVG.nvgFontFace(ctx, Style.FontSans);
            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgText(ctx, Round(x + splitWidth + (w - sfw) / 2), Round(y + (h - sfh) / 2), text);
            NanoVG.nvgFill(ctx);

            NanoVG.nvgRestore(ctx);
        }

        private static void DrawConnectedRect(NVGcontext ctx, float x, float y, float w, float h, SConnectedSide connection)
        {
            var tl = connection.HasFlag(SConnectedSide.Top) || connection.HasFlag(SConnectedSide.Left) ? 0 : Style.BorderRadius;
            var tr = connection.HasFlag(SConnectedSide.Top) || connection.HasFlag(SConnectedSide.Right) ? 0 : Style.BorderRadius;
            var bl = connection.HasFlag(SConnectedSide.Bottom) || connection.HasFlag(SConnectedSide.Left)
                ? 0
                : Style.BorderRadius;
            var br = connection.HasFlag(SConnectedSide.Bottom) || connection.HasFlag(SConnectedSide.Right)
                ? 0
                : Style.BorderRadius;

            NanoVG.nvgBeginPath(ctx);
            NanoVG.nvgRoundedRectVarying(ctx, x, y, w, h, tl, tr, br, bl);
            NanoVG.nvgFill(ctx);
        }

        public static void WithStyle(SStyle style, Action withStyle)
        {
            var orig = Style;
            Style = style;
            withStyle.Invoke();
            Style = orig;
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

        private static void SetPrimaryFillColor(NVGcontext ctx, SWidgetState state)
        {
            switch (state)
            {
                case SWidgetState.Default:
                    NanoVG.nvgFillColor(ctx, Style.PrimaryDefaultColor);
                    break;
                case SWidgetState.Hover:
                    NanoVG.nvgFillColor(ctx, Style.PrimaryHoverColor);
                    break;
                case SWidgetState.Focus:
                    NanoVG.nvgFillColor(ctx, Style.PrimaryFocusColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void SetPrimaryStrokeColor(NVGcontext ctx, SWidgetState state)
        {
            switch (state)
            {
                case SWidgetState.Default:
                    NanoVG.nvgStrokeColor(ctx, Style.PrimaryDefaultColor);
                    break;
                case SWidgetState.Hover:
                    NanoVG.nvgStrokeColor(ctx, Style.PrimaryHoverColor);
                    break;
                case SWidgetState.Focus:
                    NanoVG.nvgStrokeColor(ctx, Style.PrimaryFocusColor);
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
