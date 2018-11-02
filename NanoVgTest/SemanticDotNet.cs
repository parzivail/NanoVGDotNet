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

            public NvgColor FontDefaultColor { get; set; } = NanoVg.Rgba(0, 0, 0, 153);
            public NvgColor FontHoverColor { get; set; } = NanoVg.Rgba(0, 0, 0, 204);
            public NvgColor FontFocusColor { get; set; } = NanoVg.Rgba(0, 0, 0, 242);

            public float ButtonHeight { get; set; } = 36;

            public float BorderRadius { get; set; } = 4;

            public NvgColor PrimaryDefaultColor { get; set; } = NanoVg.Rgba(224, 225, 226, 255);
            public NvgColor PrimaryHoverColor { get; set; } = NanoVg.Rgba(202, 203, 205, 255);
            public NvgColor PrimaryFocusColor { get; set; } = NanoVg.Rgba(186, 187, 188, 255);
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
            PrimaryDefaultColor = NanoVg.Rgba(214, 215, 216, 255),
            PrimaryHoverColor = NanoVg.Rgba(192, 193, 195, 255),
            PrimaryFocusColor = NanoVg.Rgba(176, 177, 178, 255)
        };

        public static SStyle Style { get; set; } = StyleDefault;

        public static void SdnTextButton(NvgContext ctx, float x, float y, float w, float h, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            Button(ctx, x, y, w, h, Style.FontSans, text, state, connection);
        }

        public static void SdnIconButton(NvgContext ctx, float x, float y, float w, float h, string icon, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            Button(ctx, x, y, w, h, Style.FontIcon, icon, state);
        }

        public static void SdnCheckbox(NvgContext ctx, float x, float y, float size, bool @checked, string label,
            SWidgetState state = SWidgetState.Default)
        {
            NanoVg.Save(ctx);

            SetPrimaryStrokeColor(ctx, state);

            NanoVg.NvgStrokeWidth(ctx, 2);

            NanoVg.NvgBeginPath(ctx);
            NanoVg.NvgRoundedRect(ctx, x, y, size, size, Style.BorderRadius);
            NanoVg.Stroke(ctx);

            if (@checked)
            {
                NanoVg.NvgTextAlign(ctx, (int)NvgAlign.Top | (int)NvgAlign.Left);
                NanoVg.FontFace(ctx, Style.FontIcon);
                NanoVg.FontSize(ctx, size);

                var ib = new float[4];
                NanoVg.NvgTextBounds(ctx, 0, 0, MaterialDesignIcons.Check, ib);
                var ifw = ib[2] - ib[0];
                var ifh = ib[3] - ib[1];

                NanoVg.NvgFillColor(ctx, Style.FontDefaultColor);
                NanoVg.NvgText(ctx, Round(x + (size - ifw) / 2), Round(y + (size - ifh) / 2 + 1), MaterialDesignIcons.Check);
            }

            SetFontStyle(ctx, state);

            NanoVg.FontFace(ctx, Style.FontSans);
            var b = new float[4];
            NanoVg.NvgTextBounds(ctx, 0, 0, label, b);
            var fw = b[2] - b[0];
            var fh = b[3] - b[1];
            
            NanoVg.NvgText(ctx, Round(x + size + 10), Round(y + (size - fh) / 2 + 1), label);

            NanoVg.Restore(ctx);
        }

        private static void Button(NvgContext ctx, float x, float y, float w, float h, string font, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            NanoVg.Save(ctx);

            SetPrimaryFillColor(ctx, state);
            DrawFilledRect(ctx, x, y, w, h, connection);

            SetFontStyle(ctx, state);

            NanoVg.FontFace(ctx, font);
            var b = new float[4];
            NanoVg.NvgTextBounds(ctx, 0, 0, text, b);
            var fw = b[2] - b[0];
            var fh = b[3] - b[1];
            
            NanoVg.NvgText(ctx, Round(x + (w - fw) / 2), Round(y + (h - fh) / 2), text);

            NanoVg.Restore(ctx);
        }

        private static void TextBox(NvgContext ctx, float x, float y, float w, float h, string font, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            NanoVg.Save(ctx);

            SetPrimaryStrokeColor(ctx, state);
            DrawStrokedRect(ctx, x, y, w, h, connection);

            SetFontStyle(ctx, state);

            NanoVg.FontFace(ctx, font);
            var b = new float[4];
            NanoVg.NvgTextBounds(ctx, 0, 0, text, b);
            var fw = b[2] - b[0];
            var fh = b[3] - b[1];

            NanoVg.NvgScissor(ctx, x + 1, y + 1, w - 2, h - 2);
            
            NanoVg.NvgText(ctx, Round(x + (w - fw) / 2), Round(y + (h - fh) / 2), text);

            NanoVg.NvgResetScissor(ctx);

            NanoVg.Restore(ctx);
        }

        public static void SdnIconTextButton(NvgContext ctx, float x, float y, float w, float h, string icon, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            NanoVg.Save(ctx);

            SetPrimaryFillColor(ctx, state);
            DrawFilledRect(ctx, x, y, w, h, connection);

            SetFontStyle(ctx, state);

            NanoVg.FontFace(ctx, Style.FontIcon);
            var ib = new float[4];
            NanoVg.NvgTextBounds(ctx, 0, 0, icon, ib);
            var ifw = ib[2] - ib[0];

            NanoVg.FontFace(ctx, Style.FontSans);
            var sb = new float[4];
            NanoVg.NvgTextBounds(ctx, 0, 0, text, sb);
            var sfw = sb[2] - sb[0];
            var sfh = sb[3] - sb[1];

            var fw = ifw + sfw + Style.IconPadding;

            NanoVg.FontFace(ctx, Style.FontIcon);
            NanoVg.NvgText(ctx, Round(x + (w - fw) / 2), Round(y + (h - sfh) / 2), icon);

            NanoVg.FontFace(ctx, Style.FontSans);
            NanoVg.NvgText(ctx, Round(x + ifw + Style.IconPadding + (w - fw) / 2), Round(y + (h - sfh) / 2), text);

            NanoVg.Restore(ctx);
        }

        public static void SdnSplitIconTextButton(NvgContext ctx, float x, float y, float splitWidth, float w, float h, string icon, string text, SWidgetState state = SWidgetState.Default, SConnectedSide connection = SConnectedSide.None)
        {
            NanoVg.Save(ctx);

            WithStyle(StyleConnectedBtn, () =>
            {
                SetPrimaryFillColor(ctx, state);
                DrawFilledRect(ctx, x, y, splitWidth, h, connection | SConnectedSide.Right);
            });

            SetPrimaryFillColor(ctx, state);
            DrawFilledRect(ctx, x + splitWidth, y, w, h, connection | SConnectedSide.Left);

            SetFontStyle(ctx, state);

            NanoVg.FontFace(ctx, Style.FontIcon);
            var ib = new float[4];
            NanoVg.NvgTextBounds(ctx, 0, 0, icon, ib);
            var ifw = ib[2] - ib[0];
            var ifh = ib[3] - ib[1];

            NanoVg.FontFace(ctx, Style.FontSans);
            var sb = new float[4];
            NanoVg.NvgTextBounds(ctx, 0, 0, text, sb);
            var sfw = sb[2] - sb[0];
            var sfh = sb[3] - sb[1];

            NanoVg.FontFace(ctx, Style.FontIcon);
            NanoVg.NvgText(ctx, Round(x + (splitWidth - ifw) / 2), Round(y + (h - ifh) / 2), icon);

            NanoVg.FontFace(ctx, Style.FontSans);
            NanoVg.NvgText(ctx, Round(x + splitWidth + (w - sfw) / 2), Round(y + (h - sfh) / 2), text);

            NanoVg.Restore(ctx);
        }

        private static void DrawFilledRect(NvgContext ctx, float x, float y, float w, float h, SConnectedSide connection)
        {
            DrawRect(ctx, x, y, w, h, connection, true);
        }

        private static void DrawStrokedRect(NvgContext ctx, float x, float y, float w, float h, SConnectedSide connection)
        {
            DrawRect(ctx, x, y, w, h, connection, false);
        }

        private static void DrawRect(NvgContext ctx, float x, float y, float w, float h, SConnectedSide connection, bool fill)
        {
            var tl = connection.HasFlag(SConnectedSide.Top) || connection.HasFlag(SConnectedSide.Left) ? 0 : Style.BorderRadius;
            var tr = connection.HasFlag(SConnectedSide.Top) || connection.HasFlag(SConnectedSide.Right) ? 0 : Style.BorderRadius;
            var bl = connection.HasFlag(SConnectedSide.Bottom) || connection.HasFlag(SConnectedSide.Left)
                ? 0
                : Style.BorderRadius;
            var br = connection.HasFlag(SConnectedSide.Bottom) || connection.HasFlag(SConnectedSide.Right)
                ? 0
                : Style.BorderRadius;

            NanoVg.NvgBeginPath(ctx);
            NanoVg.NvgRoundedRectVarying(ctx, x, y, w, h, tl, tr, br, bl);
            if (fill)
                NanoVg.NvgFill(ctx);
            else
                NanoVg.Stroke(ctx);
        }

        public static void WithStyle(SStyle style, Action withStyle)
        {
            var orig = Style;
            Style = style;
            withStyle.Invoke();
            Style = orig;
        }

        private static void SetFontStyle(NvgContext ctx, SWidgetState state)
        {
            NanoVg.FontSize(ctx, Style.FontSize);
            NanoVg.NvgTextAlign(ctx, (int)NvgAlign.Top | (int)NvgAlign.Left);
            SetFontColor(ctx, state);
        }

        private static float Round(float x)
        {
            return (float)Math.Round(x);
        }

        private static void SetPrimaryFillColor(NvgContext ctx, SWidgetState state)
        {
            switch (state)
            {
                case SWidgetState.Default:
                    NanoVg.NvgFillColor(ctx, Style.PrimaryDefaultColor);
                    break;
                case SWidgetState.Hover:
                    NanoVg.NvgFillColor(ctx, Style.PrimaryHoverColor);
                    break;
                case SWidgetState.Focus:
                    NanoVg.NvgFillColor(ctx, Style.PrimaryFocusColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void SetPrimaryStrokeColor(NvgContext ctx, SWidgetState state)
        {
            switch (state)
            {
                case SWidgetState.Default:
                    NanoVg.NvgStrokeColor(ctx, Style.PrimaryDefaultColor);
                    break;
                case SWidgetState.Hover:
                    NanoVg.NvgStrokeColor(ctx, Style.PrimaryHoverColor);
                    break;
                case SWidgetState.Focus:
                    NanoVg.NvgStrokeColor(ctx, Style.PrimaryFocusColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void SetFontColor(NvgContext ctx, SWidgetState state)
        {
            switch (state)
            {
                case SWidgetState.Default:
                    NanoVg.NvgFillColor(ctx, Style.FontDefaultColor);
                    break;
                case SWidgetState.Hover:
                    NanoVg.NvgFillColor(ctx, Style.FontHoverColor);
                    break;
                case SWidgetState.Focus:
                    NanoVg.NvgFillColor(ctx, Style.FontFocusColor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
