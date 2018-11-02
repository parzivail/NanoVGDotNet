
//
// Copyright (c) 2009-2013 Mikko Mononen memon@inside.org
//
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
//

/*
 * Por to C#
 * Copyright (c) 2016 Miguel A. Guirado L. https://sites.google.com/site/bitiopia/
 * 
 * 	NanoVG.net is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  any later version.
 *
 *  NanoVG.net is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with NanoVG.net.  If not, see <http://www.gnu.org/licenses/>. See
 *  the file lgpl-3.0.txt for more details.
 */

//#define ONLY_FOR_DEBUG

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using FontStashDotNet;

namespace NanoVGDotNet
{
    public delegate int RenderCreateHandler(object uprt);
    public delegate int RenderCreateTextureHandler(object uptr, int type, int w, int h, int imageFlags, byte[] data);
    public delegate int RenderCreateTextureHandler2(object uptr, int type, int w, int h, int imageFlags, Bitmap bmp);
    public delegate void RenderViewportHandler(object uptr, int width, int height, float devicePixelRatio);
    public delegate void RenderFlushHandler(object uptr, NvGcompositeOperationState compositeOperation);
    public delegate void RenderFillHandler(object uptr, ref NvGpaint paint, ref NvGscissor scissor, float fringe, float[] bounds, NvGpath[] paths, int npaths);
    public delegate void RenderStrokeHandler(object uptr, ref NvGpaint paint, ref NvGscissor scissor, float fringe, float strokeWidth, NvGpath[] paths, int npaths);
    public delegate void RenderTrianglesHandler(object uptr, ref NvGpaint paint, ref NvGscissor scissor,
                                                NvGvertex[] verts, int nverts);
    public delegate int RenderUpdateTextureHandler(object uptr, int image, int x, int y, int w, int h, byte[] data);
    public delegate int RenderGetTextureSizeHandler(object uptr, int image, ref int w, ref int h);
    public delegate int RenderDeleteTexture(object uptr, int image);
    public delegate void RenderDelete(object uptr);
    public delegate void RenderCancel(object uptr);

    public static class NanoVg
    {
        public const float NvgPi = 3.14159265358979323846264338327f;

        private const int NvgInitFontimageSize = 512;
        private const int NvgMaxFontimageSize = 2048;
        public const int NvgMaxFontimages = 4;

        private const int NvgInitCommandsSize = 256;
        private const int NvgInitPointsSize = 128;
        private const int NvgInitPathsSize = 16;
        private const int NvgInitVertsSize = 256;
        public const int NvgMaxStates = 32;
        // Length proportional to radius of a cubic bezier handle for 90deg arcs.
        private const float NvgKappa90 = 0.5522847493f;

        //if defined NANOVG_GL2_IMPLEMENTATION
        public const int NanovgGlUniformarraySize = 11;

        private static int NVG_COUNTOF(int arr)
        {
            //return (sizeof(arr) / sizeof(0[arr]))
            throw new Exception("int NVG_COUNTOF(int arr)");
        }

        private static float nvg__sqrtf(float a)
        {
            return (float)Math.Sqrt(a);
        }

        private static float nvg__modf(float a, float b)
        {
            return a % b;
        }

        private static float nvg__sinf(float a)
        {
            return (float)Math.Sin(a);
        }

        private static float nvg__cosf(float a)
        {
            return (float)Math.Cos(a);
        }

        private static float nvg__tanf(float a)
        {
            return (float)Math.Tan(a);
        }

        private static float nvg__atan2f(float a, float b)
        {
            return (float)Math.Atan2(a, b);
        }

        private static float nvg__acosf(float a)
        {
            return (float)Math.Acos(a);
        }

        private static int nvg__mini(int a, int b)
        {
            return a < b ? a : b;
        }

        private static int nvg__maxi(int a, int b)
        {
            return a > b ? a : b;
        }

        private static int nvg__clampi(int a, int mn, int mx)
        {
            return a < mn ? mn : (a > mx ? mx : a);
        }

        private static float nvg__minf(float a, float b)
        {
            return a < b ? a : b;
        }

        private static float nvg__maxf(float a, float b)
        {
            return a > b ? a : b;
        }

        private static float nvg__absf(float a)
        {
            return a >= 0.0f ? a : -a;
        }

        private static float nvg__signf(float a)
        {
            return a >= 0.0f ? 1.0f : -1.0f;
        }

        private static float nvg__clampf(float a, float mn, float mx)
        {
            return a < mn ? mn : (a > mx ? mx : a);
        }

        private static float nvg__cross(float dx0, float dy0, float dx1, float dy1)
        {
            return dx1 * dy0 - dx0 * dy1;
        }


        private static float nvg__normalize(ref float x, ref float y)
        {
            var d = nvg__sqrtf(x * x + y * y);
            if (d > 1e-6f)
            {
                var id = 1.0f / d;
                x *= id;
                y *= id;
            }
            return d;
        }

        private static void nvg__deletePathCache(ref NvGpathCache c)
        {
            c.Points = null;
            c.Paths = null;
            c.Verts = null;
        }

        private static void nvg__allocPathCache(out NvGpathCache c)
        {
            c = new NvGpathCache();
            c.Points = new NvGpoint[NvgInitPointsSize];
            c.Npoints = 0;
            c.Cpoints = NvgInitPointsSize;

            c.Paths = new NvGpath[NvgInitPathsSize];
            c.Npaths = 0;
            c.Cpaths = NvgInitPathsSize;

            c.Verts = new NvGvertex[NvgInitVertsSize];
            c.Nverts = 0;
            c.Cverts = NvgInitVertsSize;
        }

        private static void nvg__setDevicePixelRatio(ref NvGcontext ctx, float ratio)
        {
            ctx.TessTol = 0.25f / ratio;
            ctx.DistTol = 0.01f / ratio;
            ctx.FringeWidth = 1.0f / ratio;
            ctx.DevicePxRatio = ratio;
        }

        private static NvGcompositeOperationState nvg__compositeOperationState(int op)
        {
            int sfactor = 0, dfactor = 0;

            if (op == (int)NvgCompositeOperation.SourceOver)
            {
                sfactor = (int)NvgBlendFactor.One;
                dfactor = (int)NvgBlendFactor.OneMinusSrcAlpha;
            }
            else if (op == (int)NvgCompositeOperation.SourceIn)
            {
                sfactor = (int)NvgBlendFactor.DstAlpha;
                dfactor = (int)NvgBlendFactor.Zero;
            }
            else if (op == (int)NvgCompositeOperation.SourceOut)
            {
                sfactor = (int)NvgBlendFactor.OneMinusDstAlpha;
                dfactor = (int)NvgBlendFactor.Zero;
            }
            else if (op == (int)NvgCompositeOperation.Atop)
            {
                sfactor = (int)NvgBlendFactor.DstAlpha;
                dfactor = (int)NvgBlendFactor.OneMinusSrcAlpha;
            }
            else if (op == (int)NvgCompositeOperation.DestinationOver)
            {
                sfactor = (int)NvgBlendFactor.OneMinusDstAlpha;
                dfactor = (int)NvgBlendFactor.One;
            }
            else if (op == (int)NvgCompositeOperation.DestinationIn)
            {
                sfactor = (int)NvgBlendFactor.Zero;
                dfactor = (int)NvgBlendFactor.SrcAlpha;
            }
            else if (op == (int)NvgCompositeOperation.DestinationOut)
            {
                sfactor = (int)NvgBlendFactor.Zero;
                dfactor = (int)NvgBlendFactor.OneMinusSrcAlpha;
            }
            else if (op == (int)NvgCompositeOperation.DestinationAtop)
            {
                sfactor = (int)NvgBlendFactor.OneMinusDstAlpha;
                dfactor = (int)NvgBlendFactor.SrcAlpha;
            }
            else if (op == (int)NvgCompositeOperation.Lighter)
            {
                sfactor = (int)NvgBlendFactor.One;
                dfactor = (int)NvgBlendFactor.One;
            }
            else if (op == (int)NvgCompositeOperation.Copy)
            {
                sfactor = (int)NvgBlendFactor.One;
                dfactor = (int)NvgBlendFactor.Zero;
            }
            else if (op == (int)NvgCompositeOperation.Xor)
            {
                sfactor = (int)NvgBlendFactor.OneMinusDstAlpha;
                dfactor = (int)NvgBlendFactor.OneMinusSrcAlpha;
            }

            NvGcompositeOperationState state;
            state.SrcRgb = sfactor;
            state.DstRgb = dfactor;
            state.SrcAlpha = sfactor;
            state.DstAlpha = dfactor;
            return state;
        }

        private static NvGstate nvg__getState(NvGcontext ctx)
        {
            return ctx.States[ctx.Nstates - 1];
        }

        // State setting
        public static void NvgFontSize(NvGcontext ctx, float size)
        {
            var state = nvg__getState(ctx);
            state.FontSize = size;
        }

        public static void NvgFontBlur(NvGcontext ctx, float blur)
        {
            var state = nvg__getState(ctx);
            state.FontBlur = blur;
        }

        public static void NvgFontFace(NvGcontext ctx, string font)
        {
            var state = nvg__getState(ctx);
            state.FontId = FontStash.fonsGetFontByName(ctx.Fs, font);
        }

        public static NvGcolor NvgRgba(byte r, byte g, byte b, byte a)
        {
            var color = default(NvGcolor);
            // Use longer initialization to suppress warning.
            color.R = r / 255.0f;
            color.G = g / 255.0f;
            color.B = b / 255.0f;
            color.A = a / 255.0f;

            return color;
        }

        private static NvGcolor NvgRgbAf(float r, float g, float b, float a)
        {
            var color = default(NvGcolor);
            // Use longer initialization to suppress warning.
            color.R = r;
            color.G = g;
            color.B = b;
            color.A = a;
            return color;
        }

        private static float nvg__getAverageScale(float[] t)
        {
            var sx = (float)Math.Sqrt(t[0] * t[0] + t[2] * t[2]);
            var sy = (float)Math.Sqrt(t[1] * t[1] + t[3] * t[3]);
            return (sx + sy) * 0.5f;
        }

        private static int nvg__curveDivs(float r, float arc, float tol)
        {
            var da = (float)Math.Acos(r / (r + tol)) * 2.0f;
            return nvg__maxi(2, (int)Math.Ceiling(arc / da));
        }

        private static void nvg__buttCapStart(NvGvertex[] dst, ref int idst, NvGpoint p,
                                      float dx, float dy, float w, float d, float aa)
        {
            var px = p.X - dx * d;
            var py = p.Y - dy * d;
            var dlx = dy;
            var dly = -dx;
            nvg__vset(ref dst[idst], px + dlx * w - dx * aa, py + dly * w - dy * aa, 0, 0);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w - dx * aa, py - dly * w - dy * aa, 1, 0);
            idst++;
            nvg__vset(ref dst[idst], px + dlx * w, py + dly * w, 0, 1);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w, py - dly * w, 1, 1);
            idst++;
        }

        private static void nvg__roundCapStart(NvGvertex[] dst, ref int idst, NvGpoint p,
                                       float dx, float dy, float w, int ncap, float aa)
        {
            int i;
            var px = p.X;
            var py = p.Y;
            var dlx = dy;
            var dly = -dx;
            //NVG_NOTUSED(aa);
            for (i = 0; i < ncap; i++)
            {
                var a = i / (float)(ncap - 1) * NvgPi;
                float ax = (float)Math.Cos(a) * w, ay = (float)Math.Sin(a) * w;
                nvg__vset(ref dst[idst], px - dlx * ax - dx * ay, py - dly * ax - dy * ay, 0, 1);
                idst++;
                nvg__vset(ref dst[idst], px, py, 0.5f, 1);
                idst++;
            }
            nvg__vset(ref dst[idst], px + dlx * w, py + dly * w, 0, 1);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w, py - dly * w, 1, 1);
            idst++;
        }

        private static void nvg__buttCapEnd(NvGvertex[] dst, ref int idst, NvGpoint p,
                                    float dx, float dy, float w, float d, float aa)
        {
            var px = p.X + dx * d;
            var py = p.Y + dy * d;
            var dlx = dy;
            var dly = -dx;
            nvg__vset(ref dst[idst], px + dlx * w, py + dly * w, 0, 1);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w, py - dly * w, 1, 1);
            idst++;
            nvg__vset(ref dst[idst], px + dlx * w + dx * aa, py + dly * w + dy * aa, 0, 0);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w + dx * aa, py - dly * w + dy * aa, 1, 0);
            idst++;
        }

        private static void nvg__roundCapEnd(NvGvertex[] dst, ref int idst, NvGpoint p,
                                     float dx, float dy, float w, int ncap, float aa)
        {
            int i;
            var px = p.X;
            var py = p.Y;
            var dlx = dy;
            var dly = -dx;
            //NVG_NOTUSED(aa);
            nvg__vset(ref dst[idst], px + dlx * w, py + dly * w, 0, 1);
            idst++;
            nvg__vset(ref dst[idst], px - dlx * w, py - dly * w, 1, 1);
            idst++;
            for (i = 0; i < ncap; i++)
            {
                var a = i / (float)(ncap - 1) * NvgPi;
                float ax = (float)Math.Cos(a) * w, ay = (float)Math.Sin(a) * w;
                nvg__vset(ref dst[idst], px, py, 0.5f, 1);
                idst++;
                nvg__vset(ref dst[idst], px - dlx * ax + dx * ay, py - dly * ax + dy * ay, 0, 1);
                idst++;
            }
        }

        private static void nvg__roundJoin(NvGvertex[] dst, ref int idst, NvGpoint p0, NvGpoint p1,
                                   float lw, float rw, float lu, float ru, int ncap, float fringe)
        {
            int i, n;
            var dlx0 = p0.Dy;
            var dly0 = -p0.Dx;
            var dlx1 = p1.Dy;
            var dly1 = -p1.Dx;
            //NVG_NOTUSED(fringe);

            if ((p1.Flags & (int)NvgPointFlags.Left) != 0)
            {
                float lx0 = 0, ly0 = 0, lx1 = 0, ly1 = 0, a0, a1;
                nvg__chooseBevel(p1.Flags & (int)NvgPointFlags.InnerBevel, p0, p1, lw,
                    ref lx0, ref ly0, ref lx1, ref ly1);
                a0 = (float)Math.Atan2(-dly0, -dlx0);
                a1 = (float)Math.Atan2(-dly1, -dlx1);
                if (a1 > a0)
                    a1 -= NvgPi * 2;

                nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1.X - dlx0 * rw, p1.Y - dly0 * rw, ru, 1);
                idst++;

                n = nvg__clampi((int)Math.Ceiling(((a0 - a1) / NvgPi) * ncap), 2, ncap);
                for (i = 0; i < n; i++)
                {
                    var u = i / (float)(n - 1);
                    var a = a0 + u * (a1 - a0);
                    var rx = (float)(p1.X + Math.Cos(a) * rw);
                    var ry = (float)(p1.Y + Math.Sin(a) * rw);
                    nvg__vset(ref dst[idst], p1.X, p1.Y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx, ry, ru, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1.X - dlx1 * rw, p1.Y - dly1 * rw, ru, 1);
                idst++;

            }
            else
            {
                float rx0 = 0, ry0 = 0, rx1 = 0, ry1 = 0, a0, a1;
                nvg__chooseBevel(p1.Flags & (int)NvgPointFlags.InnerBevel, p0, p1, -rw,
                    ref rx0, ref ry0, ref rx1, ref ry1);
                a0 = (float)Math.Atan2(dly0, dlx0);
                a1 = (float)Math.Atan2(dly1, dlx1);
                if (a1 < a0)
                    a1 += NvgPi * 2;

                nvg__vset(ref dst[idst], p1.X + dlx0 * rw, p1.Y + dly0 * rw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                idst++;

                n = nvg__clampi((int)Math.Ceiling(((a1 - a0) / NvgPi) * ncap), 2, ncap);
                for (i = 0; i < n; i++)
                {
                    var u = i / (float)(n - 1);
                    var a = a0 + u * (a1 - a0);
                    var lx = (float)(p1.X + Math.Cos(a) * lw);
                    var ly = (float)(p1.Y + Math.Sin(a) * lw);
                    nvg__vset(ref dst[idst], lx, ly, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.X, p1.Y, 0.5f, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], p1.X + dlx1 * rw, p1.Y + dly1 * rw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                idst++;

            }
            //return dst;
        }

        private static int nvg__expandStroke(NvGcontext ctx, float w, int lineCap, int lineJoin, float miterLimit)
        {
            var cache = ctx.Cache;
            NvGvertex[] verts;
            NvGvertex[] dst;
            int cverts, i, j;
            var aa = ctx.FringeWidth;
            var ncap = nvg__curveDivs(w, NvgPi, ctx.TessTol);  // Calculate divisions per half circle.

            nvg__calculateJoins(ctx, w, lineJoin, miterLimit);

            // only for debug
#if ONLY_FOR_DEBUG
			Console.WriteLine("[nvg__expandStroke()]");
			for (int cont = 0; cont < cache.npoints; cont++)
			{
				Console.Write(String.Format("Cache-Points-index {0}: ", cont));
				Console.WriteLine(String.Format("\tvalueX: {0}\tvalueY: {1} \tflags: {2}",
				                                cache.points[cont].x, cache.points[cont].y, cache.points[cont].flags));
			}
#endif


            // Calculate max vertex usage.
            cverts = 0;
            for (i = 0; i < cache.Npaths; i++)
            {
                var path = cache.Paths[i];
                var loop = (path.Closed == 0) ? 0 : 1;
                if (lineJoin == (int)NanoVGDotNet.NvgLineCap.Round)
                    cverts += (path.Count + path.Nbevel * (ncap + 2) + 1) * 2; // plus one for loop
                else
                    cverts += (path.Count + path.Nbevel * 5 + 1) * 2; // plus one for loop
                if (loop == 0)
                {
                    // space for caps
                    if (lineCap == (int)NanoVGDotNet.NvgLineCap.Round)
                    {
                        cverts += (ncap * 2 + 2) * 2;
                    }
                    else
                    {
                        cverts += (3 + 3) * 2;
                    }
                }
            }

            verts = nvg__allocTempVerts(ctx, cverts);

            if (verts == null)
                return 0;

            for (i = 0; i < cache.Npaths; i++)
            {
                var path = cache.Paths[i];
                var ipts = path.First;
                var pts = cache.Points;
                NvGpoint p0;
                var ip0 = 0;
                NvGpoint p1;
                var ip1 = 0;
                int s, e, loop;
                float dx, dy;
                var iverts = 0;

                path.Fill = null;
                path.Nfill = 0;
                path.Ifill = 0;

                // Calculate fringe or stroke
                loop = (path.Closed == 0) ? 0 : 1;
                dst = verts;
                var idst = iverts;
                path.Stroke = dst;
                path.Istroke = idst;

                if (loop != 0)
                {
                    // Looping
                    ip0 = ipts + path.Count - 1;
                    p0 = pts[ip0];
                    ip1 = ipts + 0;
                    p1 = pts[ip1];
                    s = 0;
                    e = path.Count;
                }
                else
                {
                    // Add cap
                    ip0 = ipts + 0;
                    p0 = pts[ip0];
                    ip1 = ipts + 1;
                    p1 = pts[ip1];
                    s = 1;
                    e = path.Count - 1;
                }

                if (loop == 0)
                {
                    // Add cap
                    dx = p1.X - p0.X;
                    dy = p1.Y - p0.Y;
                    nvg__normalize(ref dx, ref dy);
                    if (lineCap == (int)NanoVGDotNet.NvgLineCap.Butt)
                        nvg__buttCapStart(dst, ref idst, p0, dx, dy, w, -aa * 0.5f, aa);
                    else if (lineCap == (int)NanoVGDotNet.NvgLineCap.Butt || lineCap == (int)NanoVGDotNet.NvgLineCap.Square)
                        nvg__buttCapStart(dst, ref idst, p0, dx, dy, w, w - aa, aa);
                    else if (lineCap == (int)NanoVGDotNet.NvgLineCap.Round)
                        nvg__roundCapStart(dst, ref idst, p0, dx, dy, w, ncap, aa);

                }

                for (j = s; j < e; ++j)
                {
                    if ((p1.Flags & (int)(NvgPointFlags.Bevel | NvgPointFlags.InnerBevel)) != 0)
                    {
                        if (lineJoin == (int)NanoVGDotNet.NvgLineCap.Round)
                        {
                            nvg__roundJoin(dst, ref idst, p0, p1, w, w, 0, 1, ncap, aa);
                        }
                        else
                        {
                            nvg__bevelJoin(dst, ref idst, p0, p1, w, w, 0, 1, aa);
                        }
                    }
                    else
                    {
                        nvg__vset(ref dst[idst], p1.X + (p1.Dmx * w), p1.Y + (p1.Dmy * w), 0, 1);
                        idst++;
                        nvg__vset(ref dst[idst], p1.X - (p1.Dmx * w), p1.Y - (p1.Dmy * w), 1, 1);
                        idst++;
                    }
                    p0 = p1;
                    ip1 += 1;
                    p1 = pts[ip1];
                }

                if (loop != 0)
                {
                    // Loop it
                    nvg__vset(ref dst[idst], verts[0].X, verts[0].Y, 0, 1);
                    idst++;
                    nvg__vset(ref dst[idst], verts[1].X, verts[1].Y, 1, 1);
                    idst++;
                }
                else
                {
                    // Add cap
                    dx = p1.X - p0.X;
                    dy = p1.Y - p0.Y;
                    nvg__normalize(ref dx, ref dy);
                    if (lineCap == (int)NanoVGDotNet.NvgLineCap.Butt)
                        nvg__buttCapEnd(dst, ref idst, p1, dx, dy, w, -aa * 0.5f, aa);
                    else if (lineCap == (int)NanoVGDotNet.NvgLineCap.Butt || lineCap == (int)NanoVGDotNet.NvgLineCap.Square)
                        nvg__buttCapEnd(dst, ref idst, p1, dx, dy, w, w - aa, aa);
                    else if (lineCap == (int)NanoVGDotNet.NvgLineCap.Round)
                        nvg__roundCapEnd(dst, ref idst, p1, dx, dy, w, ncap, aa);
                }

                path.Nstroke = idst - iverts;

                verts = dst;
                iverts = idst;
            }

            //ctx.cache.verts = verts;

            return 1;
        }

        public static NvGpaint NvgBoxGradient(NvGcontext ctx,
                                              float x, float y, float w, float h, float r, float f,
                                              NvGcolor icol, NvGcolor ocol)
        {
            var p = new NvGpaint();
            //NVG_NOTUSED(ctx);
            //memset(&p, 0, sizeof(p));

            NvgTransformIdentity(p.Xform);
            p.Xform[4] = x + w * 0.5f;
            p.Xform[5] = y + h * 0.5f;

            p.Extent[0] = w * 0.5f;
            p.Extent[1] = h * 0.5f;

            p.Radius = r;

            p.Feather = nvg__maxf(1.0f, f);

            p.InnerColor = icol;
            p.OuterColor = ocol;

            return p;
        }

        public static void NvgClosePath(NvGcontext ctx)
        {
            var vals = new[] { (float)NvgCommands.Close };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void NvgPathWinding(NvGcontext ctx, int dir)
        {
            var vals = new[] { (float)NvgCommands.Winding, dir };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void NvgStroke(NvGcontext ctx)
        {
            var state = nvg__getState(ctx);
            var scale = nvg__getAverageScale(state.Xform);
            var strokeWidth = nvg__clampf(state.StrokeWidth * scale, 0.0f, 200.0f);
            var strokePaint = state.Stroke.Clone();
            NvGpath path;
            int i;

            if (strokeWidth < ctx.FringeWidth)
            {
                // If the stroke width is less than pixel size, use alpha to emulate coverage.
                // Since coverage is area, scale by alpha*alpha.
                var alpha = nvg__clampf(strokeWidth / ctx.FringeWidth, 0.0f, 1.0f);
                strokePaint.InnerColor.A *= alpha * alpha;
                strokePaint.OuterColor.A *= alpha * alpha;
                strokeWidth = ctx.FringeWidth;
            }

            // Apply global alpha
            strokePaint.InnerColor.A *= state.Alpha;
            strokePaint.OuterColor.A *= state.Alpha;

            nvg__flattenPaths(ctx);

            if (ctx.Params.EdgeAntiAlias != 0)
                nvg__expandStroke(ctx, strokeWidth * 0.5f + ctx.FringeWidth * 0.5f,
                    state.LineCap, state.LineJoin, state.MiterLimit);
            else
                nvg__expandStroke(ctx, strokeWidth * 0.5f, state.LineCap, state.LineJoin, state.MiterLimit);

            ctx.Params.RenderStroke(ctx.Params.UserPtr, ref strokePaint, ref state.Scissor, ctx.FringeWidth,
                strokeWidth, ctx.Cache.Paths, ctx.Cache.Npaths);

            // Count triangles
            for (i = 0; i < ctx.Cache.Npaths; i++)
            {
                path = ctx.Cache.Paths[i];
                ctx.StrokeTriCount += path.Nstroke - 2;
                ctx.DrawCallCount++;
            }
        }

        // State handling
        public static void NvgSave(NvGcontext ctx)
        {
            if (ctx.Nstates >= NvgMaxStates)
                return;
            if (ctx.Nstates > 0)
                //memcpy(&ctx->states[ctx->nstates], &ctx->states[ctx->nstates-1], sizeof(NVGstate));
                ctx.States[ctx.Nstates] = ctx.States[ctx.Nstates - 1].Clone();
            ctx.Nstates++;
        }

        public static void NvgRestore(NvGcontext ctx)
        {
            if (ctx.Nstates <= 1)
                return;
            ctx.Nstates--;
        }

        private static void NvgTransformPremultiply(float[] t, float[] s)
        {
            var s2 = new float[6];
            //memcpy(s2, s, sizeof(float)*6);
            Array.Copy(s, s2, 6);
            NvgTransformMultiply(s2, t);
            //memcpy(t, s2, sizeof(float)*6);
            Array.Copy(s2, t, 6);
        }

        private static void NvgTransformRotate(float[] t, float a)
        {
            float cs = nvg__cosf(a), sn = nvg__sinf(a);
            t[0] = cs;
            t[1] = sn;
            t[2] = -sn;
            t[3] = cs;
            t[4] = 0.0f;
            t[5] = 0.0f;
        }

        private static void NvgTransformTranslate(float[] t, float tx, float ty)
        {
            t[0] = 1.0f;
            t[1] = 0.0f;
            t[2] = 0.0f;
            t[3] = 1.0f;
            t[4] = tx;
            t[5] = ty;
        }

        public static float NvgDegToRad(float deg)
        {
            return deg / 180.0f * NvgPi;
        }

        // Scissoring
        public static void NvgScissor(NvGcontext ctx, float x, float y, float w, float h)
        {
            var state = nvg__getState(ctx);

            w = nvg__maxf(0.0f, w);
            h = nvg__maxf(0.0f, h);

            NvgTransformIdentity(state.Scissor.Xform);
            state.Scissor.Xform[4] = x + w * 0.5f;
            state.Scissor.Xform[5] = y + h * 0.5f;
            NvgTransformMultiply(state.Scissor.Xform, state.Xform);

            state.Scissor.Extent[0] = w * 0.5f;
            state.Scissor.Extent[1] = h * 0.5f;
        }

        private static void nvg__isectRects(float[] dst,
                                    float ax, float ay, float aw, float ah,
                                    float bx, float by, float bw, float bh)
        {
            var minx = nvg__maxf(ax, bx);
            var miny = nvg__maxf(ay, by);
            var maxx = nvg__minf(ax + aw, bx + bw);
            var maxy = nvg__minf(ay + ah, by + bh);
            dst[0] = minx;
            dst[1] = miny;
            dst[2] = nvg__maxf(0.0f, maxx - minx);
            dst[3] = nvg__maxf(0.0f, maxy - miny);
        }

        public static void NvgIntersectScissor(NvGcontext ctx, float x, float y, float w, float h)
        {
            var state = nvg__getState(ctx);
            float[] pxform = new float[6], invxorm = new float[6];
            var rect = new float[4];
            float ex, ey, tex, tey;

            // If no previous scissor has been set, set the scissor as current scissor.
            if (state.Scissor.Extent[0] < 0)
            {
                NvgScissor(ctx, x, y, w, h);
                return;
            }

            // Transform the current scissor rect into current transform space.
            // If there is difference in rotation, this will be approximation.
            //memcpy(pxform, state->scissor.xform, sizeof(float)*6);
            Array.Copy(state.Scissor.Xform, pxform, 6);
            ex = state.Scissor.Extent[0];
            ey = state.Scissor.Extent[1];
            NvgTransformInverse(invxorm, state.Xform);
            NvgTransformMultiply(pxform, invxorm);
            tex = ex * nvg__absf(pxform[0]) + ey * nvg__absf(pxform[2]);
            tey = ex * nvg__absf(pxform[1]) + ey * nvg__absf(pxform[3]);

            // Intersect rects.
            nvg__isectRects(rect, pxform[4] - tex, pxform[5] - tey, tex * 2, tey * 2, x, y, w, h);

            NvgScissor(ctx, rect[0], rect[1], rect[2], rect[3]);
        }

        public static void NvgResetScissor(NvGcontext ctx)
        {
            var state = nvg__getState(ctx);
            //memset(state->scissor.xform, 0, sizeof(state->scissor.xform));
            for (var cont = 0; cont < state.Scissor.Xform.Length; cont++)
                state.Scissor.Xform[cont] = 0f;
            state.Scissor.Extent[0] = -1.0f;
            state.Scissor.Extent[1] = -1.0f;
        }

        public static void NvgRotate(NvGcontext ctx, float angle)
        {
            var state = nvg__getState(ctx);
            var t = new float[6];
            NvgTransformRotate(t, angle);
            NvgTransformPremultiply(state.Xform, t);
        }

        public static void NvgScale(NvGcontext ctx, float x, float y)
        {
            var state = nvg__getState(ctx);
            var t = new float[6];
            NvgTransformScale(t, x, y);
            NvgTransformPremultiply(state.Xform, t);
        }

        private static void nvg__setPaintColor(ref NvGpaint p, NvGcolor color)
        {
            p = new NvGpaint();
            // la anterior línea de código equivale a "memset(p, 0, sizeof(*p));", es
            // necesario de lo contrario aparece un degradado de color no uniforme
            NvgTransformIdentity(p.Xform);
            p.Radius = 0.0f;
            p.Feather = 1.0f;
            p.InnerColor = color;
            p.OuterColor = color;
        }

        public static void NvgTranslate(NvGcontext ctx, float x, float y)
        {
            var state = nvg__getState(ctx);
            var t = new float[6];
            NvgTransformTranslate(t, x, y);
            NvgTransformPremultiply(state.Xform, t);
        }

        private static void NvgReset(NvGcontext ctx)
        {
            var state = nvg__getState(ctx);

            nvg__setPaintColor(ref state.Fill, NvgRgba(255, 255, 255, 255));
            nvg__setPaintColor(ref state.Stroke, NvgRgba(0, 0, 0, 255));
            state.CompositeOperation = nvg__compositeOperationState((int)NvgCompositeOperation.SourceOver);
            state.StrokeWidth = 1.0f;
            state.MiterLimit = 10.0f;
            state.LineCap = (int)NanoVGDotNet.NvgLineCap.Butt;
            state.LineJoin = (int)NanoVGDotNet.NvgLineCap.Miter;
            state.Alpha = 1.0f;
            NvgTransformIdentity(state.Xform);

            state.Scissor.Extent[0] = -1.0f;
            state.Scissor.Extent[1] = -1.0f;

            state.FontSize = 16.0f;
            state.LetterSpacing = 0.0f;
            state.LineHeight = 1.0f;
            state.FontBlur = 0.0f;
            state.TextAlign = (int)NvgAlign.Left | (int)NvgAlign.Baseline;
            state.FontId = 0;
        }

        public static void NvgCreateInternal(ref NvGparams params_, out NvGcontext ctx)
        {
            var fontParams = new FONSparams();
            ctx = new NvGcontext();
            int i;

            ctx.Params = params_;
            for (i = 0; i < NvgMaxFontimages; i++)
                ctx.FontImages[i] = 0;

            ctx.Commands = new float[NvgInitCommandsSize];
            ctx.Ncommands = 0;
            ctx.Ccommands = NvgInitCommandsSize;

            nvg__allocPathCache(out ctx.Cache);

            NvgSave(ctx);
            NvgReset(ctx);

            nvg__setDevicePixelRatio(ref ctx, 1.0f);

            if (ctx.Params.RenderCreate(ctx.Params.UserPtr) == 0)
                return;

            // Init font rendering
            //memset(&fontParams, 0, sizeof(fontParams));
            fontParams.width = NvgInitFontimageSize;
            fontParams.height = NvgInitFontimageSize;
            fontParams.flags = FONSflags.FONS_ZERO_TOPLEFT;
            fontParams.renderCreate = null;
            fontParams.renderUpdate = null;
            fontParams.renderDraw = null;
            fontParams.renderDelete = null;
            fontParams.userPtr = null;
            ctx.Fs = FontStash.fonsCreateInternal(ref fontParams);

            // Create font texture
            ctx.FontImages[0] = ctx.Params.RenderCreateTexture(ctx.Params.UserPtr, (int)NvgTexture.Alpha,
                fontParams.width, fontParams.height, 0, null);
            if (ctx.FontImages[0] == 0)
                throw new Exception("NanoVG.nvgCreateInternal(): Error, creating font texture");
            ctx.FontImageIdx = 0;
        }

        public static void NvgDeleteImage(NvGcontext ctx, int image)
        {
            ctx.Params.RenderDeleteTexture(ctx.Params.UserPtr, image);
        }

        private static void NvgCancelFrame(NvGcontext ctx)
        {
            ctx.Params.RenderCancel(ctx.Params.UserPtr);
        }

        private static void NvgDeleteInternal(NvGcontext ctx)
        {
            int i;
            if (ctx == null)
                return;
            //if (ctx.commands != null)
            //	free(ctx->commands);
            if (ctx.Cache != null)
                nvg__deletePathCache(ref ctx.Cache);

            if (ctx.Fs != null)
                FontStash.fonsDeleteInternal(ctx.Fs);

            for (i = 0; i < NvgMaxFontimages; i++)
            {
                if (ctx.FontImages[i] != 0)
                {
                    NvgDeleteImage(ctx, ctx.FontImages[i]);
                    ctx.FontImages[i] = 0;
                }
            }

            if (ctx.Params.RenderDelete != null)
                ctx.Params.RenderDelete(ctx.Params.UserPtr);

            //free(ctx);
            ctx = null;
        }

        public static void NvgEndFrame(NvGcontext ctx)
        {
            var state = nvg__getState(ctx);
            //Corrige(state);
            ctx.Params.RenderFlush(ctx.Params.UserPtr, state.CompositeOperation);
            if (ctx.FontImageIdx != 0)
            {
                var fontImage = ctx.FontImages[ctx.FontImageIdx];
                int i, j, iw = 0, ih = 0;
                // delete images that smaller than current one
                if (fontImage == 0)
                    return;
                NvgImageSize(ctx, fontImage, ref iw, ref ih);
                for (i = j = 0; i < ctx.FontImageIdx; i++)
                {
                    if (ctx.FontImages[i] != 0)
                    {
                        int nw = 0, nh = 0;
                        NvgImageSize(ctx, ctx.FontImages[i], ref nw, ref nh);
                        if (nw < iw || nh < ih)
                            NvgDeleteImage(ctx, ctx.FontImages[i]);
                        else
                            ctx.FontImages[j++] = ctx.FontImages[i];
                    }
                }
                // make current font image to first
                ctx.FontImages[j++] = ctx.FontImages[0];
                ctx.FontImages[0] = fontImage;
                ctx.FontImageIdx = 0;
                // clear all images after j
                for (i = j; i < NvgMaxFontimages; i++)
                    ctx.FontImages[i] = 0;
            }
        }


        // Draw
        public static void NvgBeginPath(NvGcontext ctx)
        {
            ctx.Ncommands = 0;
            nvg__clearPathCache(ctx);
        }

        private static void nvg__clearPathCache(NvGcontext ctx)
        {
            ctx.Cache.Npoints = 0;
            ctx.Cache.Npaths = 0;
        }

        private static void NvgTransformPoint(ref float dx, ref float dy, float[] t, float sx, float sy)
        {
            dx = sx * t[0] + sy * t[2] + t[4];
            dy = sx * t[1] + sy * t[3] + t[5];
        }

        private static void nvg__appendCommands(NvGcontext ctx, float[] vals, int nvals)
        {
            var state = nvg__getState(ctx);
            int i;

            if (ctx.Ncommands + nvals > ctx.Ccommands)
            {
                var ccommands = ctx.Ncommands + nvals + ctx.Ccommands / 2;
                //commands = (float*)realloc(ctx->commands, sizeof(float)*ccommands);
                Array.Resize(ref ctx.Commands, ccommands);
                ctx.Ccommands = ccommands;
            }

            if ((int)vals[0] != (int)NvgCommands.Close &&
                (int)vals[0] != (int)NvgCommands.Winding)
            {
                ctx.Commandx = vals[nvals - 2];
                ctx.Commandy = vals[nvals - 1];
            }

            // transform commands
            i = 0;
            while (i < nvals)
            {
                var cmd = (int)vals[i];
                switch (cmd)
                {
                    case (int)NvgCommands.MoveTo:
                        NvgTransformPoint(ref vals[i + 1], ref vals[i + 2], state.Xform, vals[i + 1], vals[i + 2]);
                        i += 3;
                        break;
                    case (int)NvgCommands.LineTo:
                        NvgTransformPoint(ref vals[i + 1], ref vals[i + 2], state.Xform, vals[i + 1], vals[i + 2]);
                        i += 3;
                        break;
                    case (int)NvgCommands.BezierTo:
                        NvgTransformPoint(ref vals[i + 1], ref vals[i + 2], state.Xform, vals[i + 1], vals[i + 2]);
                        NvgTransformPoint(ref vals[i + 3], ref vals[i + 4], state.Xform, vals[i + 3], vals[i + 4]);
                        NvgTransformPoint(ref vals[i + 5], ref vals[i + 6], state.Xform, vals[i + 5], vals[i + 6]);
                        i += 7;
                        break;
                    case (int)NvgCommands.Close:
                        i++;
                        break;
                    case (int)NvgCommands.Winding:
                        i += 2;
                        break;
                    default:
                        i++;
                        break;
                }
            }

            //memcpy(&ctx->commands[ctx->ncommands], vals, nvals * sizeof(float));

            Array.Copy(vals, 0, ctx.Commands, ctx.Ncommands, nvals);

            // only for debug
#if ONLY_FOR_DEBUG
			Console.WriteLine("C#");
			for (int cont = 0; cont < nvals; cont++)
			{
				Console.Write(String.Format("index {0}: ", cont));
				Console.WriteLine(String.Format("value: {0}", ctx.commands[ctx.ncommands + cont]));
			}
#endif

            ctx.Ncommands += nvals;
        }

        private static int NVG_COUNTOF(float[] arr)
        {
            return arr.Length; //(sizeof(arr) / sizeof(0[arr]));
        }

        private static void nvg__addPath(NvGcontext ctx)
        {
            NvGpath path;
            if (ctx.Cache.Npaths + 1 > ctx.Cache.Cpaths)
            {
                var cpaths = ctx.Cache.Npaths + 1 + ctx.Cache.Cpaths / 2;
                //paths = (NVGpath*)realloc(ctx->cache->paths, sizeof(NVGpath)*cpaths);
                Array.Resize(ref ctx.Cache.Paths, cpaths);
                var paths = ctx.Cache.Paths;
                if (paths == null)
                    return;
                ctx.Cache.Paths = paths;
                ctx.Cache.Cpaths = cpaths;
            }
            path = ctx.Cache.Paths[ctx.Cache.Npaths];
            if (path == null)
            {
                path = new NvGpath();
                ctx.Cache.Paths[ctx.Cache.Npaths] = path;
            }
            else
            {
                path.Closed = 0;
                path.Convex = 0;
                path.Count = 0;
                path.Fill = null;
                path.Ifill = 0;
                path.First = 0;
                path.Nbevel = 0;
                path.Nfill = 0;
                path.Nstroke = 0;
                path.Stroke = null;
                path.Istroke = 0;
                path.Winding = 0;
            }

            path.First = ctx.Cache.Npoints;
            path.Winding = (int)NvgWinding.CounterClockwise;

            ctx.Cache.Npaths++;
        }

        private static NvGpoint nvg__lastPoint(NvGcontext ctx)
        {
            if (ctx.Cache.Npoints > 0)
                return ctx.Cache.Points[ctx.Cache.Npoints - 1];
            return null;
        }

        private static NvGpath nvg__lastPath(NvGcontext ctx)
        {
            if (ctx.Cache.Npaths > 0)
                return ctx.Cache.Paths[ctx.Cache.Npaths - 1];
            return null;
        }

        private static bool nvg__ptEquals(float x1, float y1, float x2, float y2, float tol)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            return (dx * dx + dy * dy) < (tol * tol);
        }

        private static void nvg__addPoint(NvGcontext ctx, float x, float y, int flags)
        {
            NvGpoint pt;
            var path = nvg__lastPath(ctx);
            if (path == null)
                return;

            if (path.Count > 0 && ctx.Cache.Npoints > 0)
            {
                pt = nvg__lastPoint(ctx);
                if (nvg__ptEquals(pt.X, pt.Y, x, y, ctx.DistTol))
                {
                    pt.Flags |= (byte)flags;
                    return;
                }
            }

            if (ctx.Cache.Npoints + 1 > ctx.Cache.Cpoints)
            {
                var cpoints = ctx.Cache.Npoints + 1 + ctx.Cache.Cpoints / 2;
                //points = (NVGpoint*)realloc(ctx->cache->points, sizeof(NVGpoint)*cpoints);
                Array.Resize(ref ctx.Cache.Points, cpoints);
                var points = ctx.Cache.Points;

                if (points == null)
                    return;
                ctx.Cache.Points = points;
                ctx.Cache.Cpoints = cpoints;
            }

            pt = new NvGpoint();

            ctx.Cache.Points[ctx.Cache.Npoints] = pt;
            //memset(pt, 0, sizeof(*pt));
            pt.X = x;
            pt.Y = y;
            pt.Flags = (byte)flags;

            //only for debug
#if ONLY_FOR_DEBUG
			Console.WriteLine(String.Format("Added point: {0}", pt));
#endif

            ctx.Cache.Npoints++;
            path.Count++;
        }

        private static void nvg__closePath(NvGcontext ctx)
        {
            var path = nvg__lastPath(ctx);
            if (path == null)
                return;
            path.Closed = 1;
        }

        private static void nvg__pathWinding(NvGcontext ctx, int winding)
        {
            var path = nvg__lastPath(ctx);
            if (path == null)
                return;
            path.Winding = winding;
        }

        private static float nvg__triarea2(float ax, float ay, float bx, float by, float cx, float cy)
        {
            var abx = bx - ax;
            var aby = by - ay;
            var acx = cx - ax;
            var acy = cy - ay;
            return acx * aby - abx * acy;
        }

        private static float nvg__polyArea(NvGpoint[] pts, int ipts, int npts)
        {
            int i;
            float area = 0;
            for (i = 2; i < npts; i++)
            {
                var a = pts[0 + ipts];
                var b = pts[i - 1 + ipts];
                var c = pts[i + ipts];
                area += nvg__triarea2(a.X, a.Y, b.X, b.Y, c.X, c.Y);
            }
            return area * 0.5f;
        }

        private static void nvg__polyReverse(NvGpoint[] pts, int ipts, int npts)
        {
            NvGpoint tmp;
            int i = 0, j = npts - 1;
            while (i < j)
            {
                tmp = pts[i + ipts].Clone();
                pts[i + ipts] = pts[j + ipts].Clone();
                pts[j + ipts] = tmp;
                i++;
                j--;
            }
        }

        private static void nvg__tesselateBezier(NvGcontext ctx,
                                         float x1, float y1, float x2, float y2,
                                         float x3, float y3, float x4, float y4,
                                         int level, int type)
        {
            float x12, y12, x23, y23, x34, y34, x123, y123, x234, y234, x1234, y1234;
            float dx, dy, d2, d3;

            if (level > 10)
                return;

            x12 = (x1 + x2) * 0.5f;
            y12 = (y1 + y2) * 0.5f;
            x23 = (x2 + x3) * 0.5f;
            y23 = (y2 + y3) * 0.5f;
            x34 = (x3 + x4) * 0.5f;
            y34 = (y3 + y4) * 0.5f;
            x123 = (x12 + x23) * 0.5f;
            y123 = (y12 + y23) * 0.5f;

            dx = x4 - x1;
            dy = y4 - y1;
            d2 = nvg__absf(((x2 - x4) * dy - (y2 - y4) * dx));
            d3 = nvg__absf(((x3 - x4) * dy - (y3 - y4) * dx));

            if ((d2 + d3) * (d2 + d3) < ctx.TessTol * (dx * dx + dy * dy))
            {
                nvg__addPoint(ctx, x4, y4, type);
                return;
            }

            /*	if (nvg__absf(x1+x3-x2-x2) + nvg__absf(y1+y3-y2-y2) + nvg__absf(x2+x4-x3-x3) + nvg__absf(y2+y4-y3-y3) < ctx->tessTol) {
					nvg__addPoint(ctx, x4, y4, type);
				return;
			}*/

            x234 = (x23 + x34) * 0.5f;
            y234 = (y23 + y34) * 0.5f;
            x1234 = (x123 + x234) * 0.5f;
            y1234 = (y123 + y234) * 0.5f;

            nvg__tesselateBezier(ctx, x1, y1, x12, y12, x123, y123, x1234, y1234, level + 1, 0);
            nvg__tesselateBezier(ctx, x1234, y1234, x234, y234, x34, y34, x4, y4, level + 1, type);
        }

        private static void nvg__flattenPaths(NvGcontext ctx)
        {
            var cache = ctx.Cache;
            //	NVGstate* state = nvg__getState(ctx);
            NvGpoint last;
            NvGpoint p0;
            NvGpoint p1;
            var ip1 = 0;
            NvGpoint[] pts;
            var ipts = 0;
            NvGpath path;
            int i, j;
            float[] cp1;
            var icp1 = 0;
            float[] cp2;
            var icp2 = 0;
            float[] p;
            var ip = 0;
            float area;

            if (cache.Npaths > 0)
                return;

            // Flatten
            i = 0;
            while (i < ctx.Ncommands)
            {
                var cmd = (int)ctx.Commands[i];

                switch (cmd)
                {
                    case (int)NvgCommands.MoveTo:
                        nvg__addPath(ctx);
                        p = ctx.Commands;
                        ip = i + 1;
                        nvg__addPoint(ctx, p[0 + ip], p[1 + ip], (int)NvgPointFlags.Corner);
                        i += 3;
                        break;
                    case (int)NvgCommands.LineTo:
                        p = ctx.Commands;
                        ip = i + 1;
                        nvg__addPoint(ctx, p[0 + ip], p[1 + ip], (int)NvgPointFlags.Corner);
                        i += 3;
                        break;
                    case (int)NvgCommands.BezierTo:
                        last = nvg__lastPoint(ctx);
                        if (last != null)
                        {
                            cp1 = ctx.Commands;
                            icp1 = i + 1;
                            cp2 = ctx.Commands;
                            icp2 = i + 3;
                            p = ctx.Commands;
                            ip = i + 5;
                            nvg__tesselateBezier(ctx, last.X, last.Y,
                                cp1[0 + icp1], cp1[1 + icp1],
                                cp2[0 + icp2], cp2[1 + icp2],
                                p[0 + ip],
                                p[1 + ip],
                                0, (int)NvgPointFlags.Corner);
                        }
                        i += 7;
                        break;
                    case (int)NvgCommands.Close:
                        nvg__closePath(ctx);
                        i++;
                        break;
                    case (int)NvgCommands.Winding:
                        nvg__pathWinding(ctx, (int)ctx.Commands[i + 1]);
                        i += 2;
                        break;
                    default:
                        i++;
                        break;
                }
            }

            cache.Bounds[0] = cache.Bounds[1] = 1e6f;
            cache.Bounds[2] = cache.Bounds[3] = -1e6f;

            // Calculate the direction and length of line segments.
            for (j = 0; j < cache.Npaths; j++)
            {
                path = cache.Paths[j];
                pts = cache.Points;
                ipts = path.First;

                // If the first and last points are the same, remove the last, mark as closed path.
                p0 = pts[ipts + path.Count - 1];
                ip1 = 0 + ipts;
                p1 = pts[ip1];

                if (nvg__ptEquals(p0.X, p0.Y, p1.X, p1.Y, ctx.DistTol))
                {
                    path.Count--;
                    p0 = pts[ipts + path.Count - 1];
                    path.Closed = 1;
                }

                // Enforce winding.
                if (path.Count > 2)
                {
                    area = nvg__polyArea(pts, ipts, path.Count);
                    if (path.Winding == (int)NvgWinding.CounterClockwise && area < 0.0f)
                    {
                        nvg__polyReverse(pts, ipts, path.Count);
                        p0 = pts[ipts + path.Count - 1];
                        p1 = pts[ip1];
                    }
                    if (path.Winding == (int)NvgWinding.Clockwise && area > 0.0f)
                    {
                        nvg__polyReverse(pts, ipts, path.Count);
                        p0 = pts[ipts + path.Count - 1];
                        p1 = pts[ip1];
                    }
                }

                for (i = 0; i < path.Count; i++)
                {
                    // Calculate segment direction and length
                    p0.Dx = p1.X - p0.X;
                    p0.Dy = p1.Y - p0.Y;
                    p0.Len = nvg__normalize(ref p0.Dx, ref p0.Dy);
                    // Update bounds
                    cache.Bounds[0] = nvg__minf(cache.Bounds[0], p0.X);
                    cache.Bounds[1] = nvg__minf(cache.Bounds[1], p0.Y);
                    cache.Bounds[2] = nvg__maxf(cache.Bounds[2], p0.X);
                    cache.Bounds[3] = nvg__maxf(cache.Bounds[3], p0.Y);
                    // Advance
                    p0 = p1;
                    ip1 += 1;
                    p1 = pts[ip1];
                }
            }
        }

        public static void NvgTransformMultiply(float[] t, float[] s)
        {
            var t0 = t[0] * s[0] + t[1] * s[2];
            var t2 = t[2] * s[0] + t[3] * s[2];
            var t4 = t[4] * s[0] + t[5] * s[2] + s[4];
            t[1] = t[0] * s[1] + t[1] * s[3];
            t[3] = t[2] * s[1] + t[3] * s[3];
            t[5] = t[4] * s[1] + t[5] * s[3] + s[5];
            t[0] = t0;
            t[2] = t2;
            t[4] = t4;
        }

        public static void NvgLineJoin(NvGcontext ctx, int join)
        {
            var state = nvg__getState(ctx);
            state.LineJoin = join;
        }

        public static void NvgMoveTo(NvGcontext ctx, float x, float y)
        {
            var vals = new[] { (float)NvgCommands.MoveTo, x, y };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void NvgBezierTo(NvGcontext ctx, float c1X, float c1Y, float c2X, float c2Y, float x, float y)
        {
            var vals = new[] { (float)NvgCommands.BezierTo, c1X, c1Y, c2X, c2Y, x, y };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void NvgLineTo(NvGcontext ctx, float x, float y)
        {
            var vals = new[] { (float)NvgCommands.LineTo, x, y };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void NvgLineCap(NvGcontext ctx, int cap)
        {
            var state = nvg__getState(ctx);
            state.LineCap = cap;
        }

        public static void NvgFillPaint(NvGcontext ctx, NvGpaint paint)
        {
            var state = nvg__getState(ctx);
            state.Fill = paint.Clone();
            NvgTransformMultiply(state.Fill.Xform, state.Xform);
        }

        public static void NvgFillColor(NvGcontext ctx, NvGcolor color)
        {
            var state = nvg__getState(ctx);
            nvg__setPaintColor(ref state.Fill, color);
        }

        public static void NvgStrokePaint(NvGcontext ctx, NvGpaint paint)
        {
            var state = nvg__getState(ctx);
            state.Stroke = paint.Clone();
            NvgTransformMultiply(state.Stroke.Xform, state.Xform);
        }

        public static void NvgStrokeColor(NvGcontext ctx, NvGcolor color)
        {
            var state = nvg__getState(ctx);
            nvg__setPaintColor(ref state.Stroke, color);
        }

        // State setting
        public static void NvgStrokeWidth(NvGcontext ctx, float width)
        {
            var state = nvg__getState(ctx);
            state.StrokeWidth = width;
        }

        private static void nvg__vset(ref NvGvertex vtx, float x, float y, float u, float v)
        {
            vtx.X = x;
            vtx.Y = y;
            vtx.U = u;
            vtx.V = v;
        }

        private static void nvg__calculateJoins(NvGcontext ctx, float w, int lineJoin, float miterLimit)
        {
            var cache = ctx.Cache;
            int i, j;
            var iw = 0.0f;

            if (w > 0.0f)
                iw = 1.0f / w;

            // Calculate which joins needs extra vertices to append, and gather vertex count.
            for (i = 0; i < cache.Npaths; i++)
            {
                var path = cache.Paths[i];

                var ipts = path.First;
                var pts = cache.Points;

                var p0 = pts[ipts + path.Count - 1];

                var ip1 = ipts;
                var p1 = pts[ip1 + 0];

                var nleft = 0;

                path.Nbevel = 0;

                for (j = 0; j < path.Count; j++)
                {
                    float dlx0, dly0, dlx1, dly1, dmr2, cross, limit;
                    dlx0 = p0.Dy;
                    dly0 = -p0.Dx;
                    dlx1 = p1.Dy;
                    dly1 = -p1.Dx;
                    // Calculate extrusions
                    p1.Dmx = (dlx0 + dlx1) * 0.5f;
                    p1.Dmy = (dly0 + dly1) * 0.5f;
                    dmr2 = p1.Dmx * p1.Dmx + p1.Dmy * p1.Dmy;
                    if (dmr2 > 0.000001f)
                    {
                        var scale = 1.0f / dmr2;
                        if (scale > 600.0f)
                        {
                            scale = 600.0f;
                        }
                        p1.Dmx *= scale;
                        p1.Dmy *= scale;
                    }

                    // Clear flags, but keep the corner.
                    p1.Flags = (byte)(((p1.Flags &
                    (int)NvgPointFlags.Corner) != 0) ? (int)NvgPointFlags.Corner : 0);

                    // Keep track of left turns.
                    cross = p1.Dx * p0.Dy - p0.Dx * p1.Dy;
                    if (cross > 0.0f)
                    {
                        nleft++;
                        p1.Flags |= (int)NvgPointFlags.Left;
                    }

                    // Calculate if we should use bevel or miter for inner join.
                    limit = nvg__maxf(1.01f, nvg__minf(p0.Len, p1.Len) * iw);
                    if ((dmr2 * limit * limit) < 1.0f)
                        p1.Flags |= (int)NvgPointFlags.InnerBevel;

                    // Check to see if the corner needs to be beveled.
                    if ((p1.Flags & (int)NvgPointFlags.Corner) != 0)
                    {
                        if ((dmr2 * miterLimit * miterLimit) < 1.0f ||
                            lineJoin == (int)NanoVGDotNet.NvgLineCap.Bevel ||
                            lineJoin == (int)NanoVGDotNet.NvgLineCap.Round)
                        {
                            p1.Flags |= (int)NvgPointFlags.Bevel;
                        }
                    }

                    if ((p1.Flags & ((int)NvgPointFlags.Bevel | (int)NvgPointFlags.InnerBevel)) != 0)
                        path.Nbevel++;

                    p0 = p1;
                    ip1 += 1;
                    p1 = pts[ip1];
                }

                path.Convex = (nleft == path.Count) ? 1 : 0;
            }
        }

        private static NvGvertex[] nvg__allocTempVerts(NvGcontext ctx, int nverts)
        {
            if (nverts > ctx.Cache.Cverts)
            {
                var cverts = (nverts + 0xff) & ~0xff; // Round up to prevent allocations when things change just slightly.
                                                      //verts = (NVGvertex*)realloc(ctx->cache->verts, sizeof(NVGvertex)*cverts);
                Array.Resize(ref ctx.Cache.Verts, cverts);
                ctx.Cache.Cverts = cverts;
            }

            return ctx.Cache.Verts;
        }

        private static void nvg__chooseBevel(int bevel, NvGpoint[] p0, int ip0, NvGpoint[] p1, int ip1, float w,
                                     ref float x0, ref float y0, ref float x1, ref float y1)
        {
            if (bevel != 0)
            {
                x0 = p1[ip1].X + p0[ip0].Dy * w;
                y0 = p1[ip1].Y - p0[ip0].Dx * w;
                x1 = p1[ip1].X + p1[ip1].Dy * w;
                y1 = p1[ip1].Y - p1[ip1].Dx * w;
            }
            else
            {
                x0 = p1[ip1].X + p1[ip1].Dmx * w;
                y0 = p1[ip1].Y + p1[ip1].Dmy * w;
                x1 = p1[ip1].X + p1[ip1].Dmx * w;
                y1 = p1[ip1].Y + p1[ip1].Dmy * w;
            }
        }

        private static void nvg__chooseBevel(int bevel, NvGpoint p0, NvGpoint p1, float w,
                                     ref float x0, ref float y0, ref float x1, ref float y1)
        {
            if (bevel != 0)
            {
                x0 = p1.X + p0.Dy * w;
                y0 = p1.Y - p0.Dx * w;
                x1 = p1.X + p1.Dy * w;
                y1 = p1.Y - p1.Dx * w;
            }
            else
            {
                x0 = p1.X + p1.Dmx * w;
                y0 = p1.Y + p1.Dmy * w;
                x1 = p1.X + p1.Dmx * w;
                y1 = p1.Y + p1.Dmy * w;
            }
        }

        private static void nvg__bevelJoin(NvGvertex[] dst, ref int idst,
                                   NvGpoint p0, NvGpoint p1, float lw, float rw, float lu, float ru, float fringe)
        {
            float rx0 = 0, ry0 = 0, rx1 = 0, ry1 = 0;
            float lx0 = 0, ly0 = 0, lx1 = 0, ly1 = 0;
            var dlx0 = p0.Dy;
            var dly0 = -p0.Dx;
            var dlx1 = p1.Dy;
            var dly1 = -p1.Dx;
            //NVG_NOTUSED(fringe);

            if ((p1.Flags & (int)NvgPointFlags.Left) != 0)
            {
                nvg__chooseBevel(p1.Flags & (int)NvgPointFlags.InnerBevel,
                    p0, p1, lw, ref lx0, ref ly0, ref lx1, ref ly1);

                nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1.X - dlx0 * rw, p1.Y - dly0 * rw, ru, 1);
                idst++;

                if ((p1.Flags & (int)NvgPointFlags.Bevel) != 0)
                {
                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.X - dlx0 * rw, p1.Y - dly0 * rw, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.X - dlx1 * rw, p1.Y - dly1 * rw, ru, 1);
                    idst++;
                }
                else
                {
                    rx0 = p1.X - p1.Dmx * rw;
                    ry0 = p1.Y - p1.Dmy * rw;

                    nvg__vset(ref dst[idst], p1.X, p1.Y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.X - dlx0 * rw, p1.Y - dly0 * rw, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1.X, p1.Y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.X - dlx1 * rw, p1.Y - dly1 * rw, ru, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1.X - dlx1 * rw, p1.Y - dly1 * rw, ru, 1);
                idst++;

            }
            else
            {
                nvg__chooseBevel(p1.Flags & (int)NvgPointFlags.InnerBevel,
                    p0, p1, -rw, ref rx0, ref ry0, ref rx1, ref ry1);

                nvg__vset(ref dst[idst], p1.X + dlx0 * lw, p1.Y + dly0 * lw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                idst++;

                if ((p1.Flags & (int)NvgPointFlags.Bevel) != 0)
                {
                    nvg__vset(ref dst[idst], p1.X + dlx0 * lw, p1.Y + dly0 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1.X + dlx1 * lw, p1.Y + dly1 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                    idst++;
                }
                else
                {
                    lx0 = p1.X + p1.Dmx * lw;
                    ly0 = p1.Y + p1.Dmy * lw;

                    nvg__vset(ref dst[idst], p1.X + dlx0 * lw, p1.Y + dly0 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.X, p1.Y, 0.5f, 1);
                    idst++;

                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1.X + dlx1 * lw, p1.Y + dly1 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1.X, p1.Y, 0.5f, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], p1.X + dlx1 * lw, p1.Y + dly1 * lw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                idst++;
            }

            //return dst[idst];
        }

        private static void nvg__bevelJoin(NvGvertex[] dst, ref int idst,
                                   NvGpoint[] p0, int ip0, NvGpoint[] p1, int ip1,
                                   float lw, float rw, float lu, float ru, float fringe)
        {
            float rx0 = 0, ry0 = 0, rx1 = 0, ry1 = 0;
            float lx0 = 0, ly0 = 0, lx1 = 0, ly1 = 0;
            var dlx0 = p0[ip0].Dy;
            var dly0 = -p0[ip0].Dx;
            var dlx1 = p1[ip1].Dy;
            var dly1 = -p1[ip1].Dx;
            //NVG_NOTUSED(fringe);

            if ((p1[ip1].Flags & (int)NvgPointFlags.Left) != 0)
            {
                nvg__chooseBevel(p1[ip1].Flags & (int)NvgPointFlags.InnerBevel,
                    p0, ip0, p1, ip1, lw, ref lx0, ref ly0, ref lx1, ref ly1);

                nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1[ip1].X - dlx0 * rw, p1[ip1].Y - dly0 * rw, ru, 1);
                idst++;

                if ((p1[ip1].Flags & (int)NvgPointFlags.Bevel) != 0)
                {
                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].X - dlx0 * rw, p1[ip1].Y - dly0 * rw, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].X - dlx1 * rw, p1[ip1].Y - dly1 * rw, ru, 1);
                    idst++;
                }
                else
                {
                    rx0 = p1[ip1].X - p1[ip1].Dmx * rw;
                    ry0 = p1[ip1].Y - p1[ip1].Dmy * rw;

                    nvg__vset(ref dst[idst], p1[ip1].X, p1[ip1].Y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].X - dlx0 * rw, p1[ip1].Y - dly0 * rw, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1[ip1].X, p1[ip1].Y, 0.5f, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].X - dlx1 * rw, p1[ip1].Y - dly1 * rw, ru, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], lx1, ly1, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], p1[ip1].X - dlx1 * rw, p1[ip1].Y - dly1 * rw, ru, 1);
                idst++;

            }
            else
            {
                nvg__chooseBevel(p1[ip1].Flags & (int)NvgPointFlags.InnerBevel,
                    p0, ip0, p1, ip1, -rw, ref rx0, ref ry0, ref rx1, ref ry1);

                nvg__vset(ref dst[idst], p1[ip1].X + dlx0 * lw, p1[ip1].Y + dly0 * lw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                idst++;

                if ((p1[ip1].Flags & (int)NvgPointFlags.Bevel) != 0)
                {
                    nvg__vset(ref dst[idst], p1[ip1].X + dlx0 * lw, p1[ip1].Y + dly0 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx0, ry0, ru, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1[ip1].X + dlx1 * lw, p1[ip1].Y + dly1 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                    idst++;
                }
                else
                {
                    lx0 = p1[ip1].X + p1[ip1].Dmx * lw;
                    ly0 = p1[ip1].Y + p1[ip1].Dmy * lw;

                    nvg__vset(ref dst[idst], p1[ip1].X + dlx0 * lw, p1[ip1].Y + dly0 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].X, p1[ip1].Y, 0.5f, 1);
                    idst++;

                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], lx0, ly0, lu, 1);
                    idst++;

                    nvg__vset(ref dst[idst], p1[ip1].X + dlx1 * lw, p1[ip1].Y + dly1 * lw, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], p1[ip1].X, p1[ip1].Y, 0.5f, 1);
                    idst++;
                }

                nvg__vset(ref dst[idst], p1[ip1].X + dlx1 * lw, p1[ip1].Y + dly1 * lw, lu, 1);
                idst++;
                nvg__vset(ref dst[idst], rx1, ry1, ru, 1);
                idst++;
            }

            //return dst[idst];
        }

        private static int nvg__expandFill(NvGcontext ctx, float w, int lineJoin, float miterLimit)
        {
            var cache = ctx.Cache;
            NvGvertex[] verts;
            var iverts = 0;
            NvGvertex[] dst;
            var idst = 0;
            int cverts, i, j;
            var convex = false;
            var aa = ctx.FringeWidth;
            var fringe = w > 0.0f;

            nvg__calculateJoins(ctx, w, lineJoin, miterLimit);

            // Calculate max vertex usage.
            cverts = 0;
            for (i = 0; i < cache.Npaths; i++)
            {
                var path = cache.Paths[i];
                cverts += path.Count + path.Nbevel + 1;
                if (fringe)
                    cverts += (path.Count + path.Nbevel * 5 + 1) * 2; // plus one for loop
            }

            verts = nvg__allocTempVerts(ctx, cverts);
            if (verts == null)
                return 0;

            convex = cache.Npaths == 1 && cache.Paths[0].Convex != 0;

            NvGpath path2;

            for (i = 0; i < cache.Npaths; i++)
            {
                path2 = cache.Paths[i];
                var pts = cache.Points;
                var ipts = path2.First;
                NvGpoint p0;
                var ip0 = 0;
                NvGpoint p1;
                var ip1 = 0;
                float rw, lw, woff;
                float ru, lu;

                // Calculate shape vertices.
                woff = 0.5f * aa;
                dst = verts;
                idst = iverts;
                path2.Fill = dst;
                path2.Ifill = idst;

                if (fringe)
                {
                    // Looping
                    ip0 = ipts + path2.Count - 1;
                    p0 = pts[ip0];
                    ip1 = ipts + 0;
                    p1 = pts[ip1];

                    for (j = 0; j < path2.Count; ++j)
                    {
                        if ((p1.Flags & (int)NvgPointFlags.Bevel) != 0)
                        {
                            var dlx0 = p0.Dy;
                            var dly0 = -p0.Dx;
                            var dlx1 = p1.Dy;
                            var dly1 = -p1.Dx;
                            if ((p1.Flags & (int)NvgPointFlags.Left) != 0)
                            {
                                var lx = p1.X + p1.Dmx * woff;
                                var ly = p1.Y + p1.Dmy * woff;
                                nvg__vset(ref dst[idst], lx, ly, 0.5f, 1);
                                idst++;
                            }
                            else
                            {
                                var lx0 = p1.X + dlx0 * woff;
                                var ly0 = p1.Y + dly0 * woff;
                                var lx1 = p1.X + dlx1 * woff;
                                var ly1 = p1.Y + dly1 * woff;
                                nvg__vset(ref dst[idst], lx0, ly0, 0.5f, 1);
                                idst++;
                                nvg__vset(ref dst[idst], lx1, ly1, 0.5f, 1);
                                idst++;
                            }
                        }
                        else
                        {
                            nvg__vset(ref dst[idst], p1.X + (p1.Dmx * woff), p1.Y + (p1.Dmy * woff), 0.5f, 1);
                            idst++;
                        }
                        p0 = p1;
                        ip1 += 1;
                        p1 = pts[ip1];
                    }
                }
                else
                {
                    for (j = 0; j < path2.Count; ++j)
                    {
                        nvg__vset(ref dst[idst], pts[j + ipts].X, pts[j + ipts].Y, 0.5f, 1);
                        idst++;
                    }
                }

                path2.Nfill = idst - iverts;
                verts = dst;
                iverts = idst;

                // Calculate fringe (Calcula flecos)
                if (fringe)
                {
                    lw = w + woff;
                    rw = w - woff;
                    lu = 0;
                    ru = 1;
                    idst = iverts;
                    dst = verts;
                    path2.Stroke = dst;
                    path2.Istroke = idst;

                    // Create only half a fringe for convex shapes so that
                    // the shape can be rendered without stenciling.
                    if (convex)
                    {
                        lw = woff;  // This should generate the same vertex as fill inset above.
                        lu = 0.5f;  // Set outline fade at middle.
                    }

                    // Looping
                    ip0 = ipts + path2.Count - 1;
                    p0 = pts[ip0];
                    ip1 = 0 + ipts;
                    p1 = pts[ip1];

                    for (j = 0; j < path2.Count; ++j)
                    {
                        if ((p1.Flags &
                            ((int)NvgPointFlags.Bevel | (int)NvgPointFlags.InnerBevel)) != 0)
                        {
                            nvg__bevelJoin(dst, ref idst, p0, p1, lw, rw, lu, ru, ctx.FringeWidth);
                        }
                        else
                        {
                            nvg__vset(ref dst[idst], p1.X + (p1.Dmx * lw), p1.Y + (p1.Dmy * lw), lu, 1);
                            idst++;
                            nvg__vset(ref dst[idst], p1.X - (p1.Dmx * rw), p1.Y - (p1.Dmy * rw), ru, 1);
                            idst++;
                        }
                        p0 = p1;
                        ip1 += 1;
                        p1 = pts[ip1];
                    }

                    // Loop it
                    nvg__vset(ref dst[idst], verts[0 + iverts].X, verts[0 + iverts].Y, lu, 1);
                    idst++;
                    nvg__vset(ref dst[idst], verts[1 + iverts].X, verts[1 + iverts].Y, ru, 1);
                    idst++;

                    path2.Nstroke = idst - iverts;
                    iverts = idst;
                    verts = dst;
                }
                else
                {
                    path2.Stroke = null;
                    path2.Nstroke = 0;
                }

#if ONLY_FOR_DEBUG
				for(int cont=path2.istroke, cont2=0; cont < path2.nstroke; cont++, cont2++)
					Console.WriteLine(String.Format("Index-stroke[{0}] x={1} y={2} u={3} v={4}",
						cont2, path2.stroke[cont].x,
						path2.stroke[cont].y,
						path2.stroke[cont].u,
						path2.stroke[cont].v));
#endif
            }

            //ctx.cache.verts = verts;

            return 1;
        }

        public static void NvgTransformScale(float[] t, float sx, float sy)
        {
            t[0] = sx;
            t[1] = 0.0f;
            t[2] = 0.0f;
            t[3] = sy;
            t[4] = 0.0f;
            t[5] = 0.0f;
        }

        public static int NvgTransformInverse(float[] inv, float[] t)
        {
            double invdet, det = (double)t[0] * t[3] - (double)t[2] * t[1];
            if (det > -1e-6 && det < 1e-6)
            {
                NvgTransformIdentity(inv);
                return 0;
            }
            invdet = 1.0 / det;
            inv[0] = (float)(t[3] * invdet);
            inv[2] = (float)(-t[2] * invdet);
            inv[4] = (float)(((double)t[2] * t[5] - (double)t[3] * t[4]) * invdet);
            inv[1] = (float)(-t[1] * invdet);
            inv[3] = (float)(t[0] * invdet);
            inv[5] = (float)(((double)t[1] * t[4] - (double)t[0] * t[5]) * invdet);
            return 1;
        }

        public static void NvgFill(NvGcontext ctx)
        {
            var state = nvg__getState(ctx);
            NvGpath path;
            var fillPaint = state.Fill.Clone();
            int i;

            nvg__flattenPaths(ctx);

            if (ctx.Params.EdgeAntiAlias != 0)
                nvg__expandFill(ctx, ctx.FringeWidth, (int)NanoVGDotNet.NvgLineCap.Miter, 2.4f);
            else
                nvg__expandFill(ctx, 0.0f, (int)NanoVGDotNet.NvgLineCap.Miter, 2.4f);

            // Apply global alpha
            fillPaint.InnerColor.A *= state.Alpha;
            fillPaint.OuterColor.A *= state.Alpha;

            ctx.Params.RenderFill(ctx.Params.UserPtr, ref fillPaint, ref state.Scissor, ctx.FringeWidth,
                ctx.Cache.Bounds, ctx.Cache.Paths, ctx.Cache.Npaths);

            // Count triangles
            for (i = 0; i < ctx.Cache.Npaths; i++)
            {
                path = ctx.Cache.Paths[i];
                ctx.FillTriCount += path.Nfill - 2;
                ctx.FillTriCount += path.Nstroke - 2;
                ctx.DrawCallCount += 2;
            }
        }

        public static void NvgRect(NvGcontext ctx, float x, float y, float w, float h)
        {
            float[] vals =
            {
                (float)NvgCommands.MoveTo, x, y,
                (float)NvgCommands.LineTo, x, y + h,
                (float)NvgCommands.LineTo, x + w, y + h,
                (float)NvgCommands.LineTo, x + w, y,
                (float)NvgCommands.Close
            };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        private static float nvg__distPtSeg(float x, float y, float px, float py, float qx, float qy)
        {
            float pqx, pqy, dx, dy, d, t;
            pqx = qx - px;
            pqy = qy - py;
            dx = x - px;
            dy = y - py;
            d = pqx * pqx + pqy * pqy;
            t = pqx * dx + pqy * dy;
            if (d > 0) t /= d;
            if (t < 0) t = 0;
            else if (t > 1) t = 1;
            dx = px + t * pqx - x;
            dy = py + t * pqy - y;
            return dx * dx + dy * dy;
        }

        public static void NvgArcTo(NvGcontext ctx, float x1, float y1, float x2, float y2, float radius)
        {
            float x0 = ctx.Commandx;
            float y0 = ctx.Commandy;
            float dx0, dy0, dx1, dy1, a, d, cx, cy, a0, a1;
            int dir;

            if (ctx.Ncommands == 0)
            {
                return;
            }

            // Handle degenerate cases.
            if (nvg__ptEquals(x0, y0, x1, y1, ctx.DistTol) ||
                nvg__ptEquals(x1, y1, x2, y2, ctx.DistTol) ||
                nvg__distPtSeg(x1, y1, x0, y0, x2, y2) < ctx.DistTol * ctx.DistTol ||
                radius < ctx.DistTol)
            {
                NvgLineTo(ctx, x1, y1);
                return;
            }

            // Calculate tangential circle to lines (x0,y0)-(x1,y1) and (x1,y1)-(x2,y2).
            dx0 = x0 - x1;
            dy0 = y0 - y1;
            dx1 = x2 - x1;
            dy1 = y2 - y1;
            nvg__normalize(ref dx0, ref dy0);
            nvg__normalize(ref dx1, ref dy1);
            a = nvg__acosf(dx0 * dx1 + dy0 * dy1);
            d = radius / nvg__tanf(a / 2.0f);

            //	printf("a=%f° d=%f\n", a/NVG_PI*180.0f, d);

            if (d > 10000.0f)
            {
                NvgLineTo(ctx, x1, y1);
                return;
            }

            if (nvg__cross(dx0, dy0, dx1, dy1) > 0.0f)
            {
                cx = x1 + dx0 * d + dy0 * radius;
                cy = y1 + dy0 * d + -dx0 * radius;
                a0 = nvg__atan2f(dx0, -dy0);
                a1 = nvg__atan2f(-dx1, dy1);
                dir = (int)NvgWinding.Clockwise;
                //		printf("CW c=(%f, %f) a0=%f° a1=%f°\n", cx, cy, a0/NVG_PI*180.0f, a1/NVG_PI*180.0f);
            }
            else
            {
                cx = x1 + dx0 * d + -dy0 * radius;
                cy = y1 + dy0 * d + dx0 * radius;
                a0 = nvg__atan2f(-dx0, dy0);
                a1 = nvg__atan2f(dx1, -dy1);
                dir = (int)NvgWinding.CounterClockwise;
                //		printf("CCW c=(%f, %f) a0=%f° a1=%f°\n", cx, cy, a0/NVG_PI*180.0f, a1/NVG_PI*180.0f);
            }

            NvgArc(ctx, cx, cy, radius, a0, a1, dir);
        }

        public static void NvgQuadTo(NvGcontext ctx, float cx, float cy, float x, float y)
        {
            float x0 = ctx.Commandx;
            float y0 = ctx.Commandy;
            float[] vals = { (int)NvgCommands.BezierTo,
                x0 + 2.0f/3.0f*(cx - x0), y0 + 2.0f/3.0f*(cy - y0),
                x + 2.0f/3.0f*(cx - x), y + 2.0f/3.0f*(cy - y),
                x, y
            };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void NvgArc(NvGcontext ctx, float cx, float cy, float r, float a0, float a1, int dir)
        {
            float a = 0, da = 0, hda = 0, kappa = 0;
            float dx = 0, dy = 0, x = 0, y = 0, tanx = 0, tany = 0;
            float px = 0, py = 0, ptanx = 0, ptany = 0;
            var vals = new float[3 + 5 * 7 + 100];
            int i, ndivs, nvals;
            var move = ctx.Ncommands > 0 ? (int)NvgCommands.LineTo : (int)NvgCommands.MoveTo;

            // Clamp angles
            da = a1 - a0;
            if (dir == (int)NvgWinding.Clockwise)
            {
                if (nvg__absf(da) >= NvgPi * 2)
                {
                    da = NvgPi * 2;
                }
                else
                {
                    while (da < 0.0f)
                        da += NvgPi * 2;
                }
            }
            else
            {
                if (nvg__absf(da) >= NvgPi * 2)
                {
                    da = -NvgPi * 2;
                }
                else
                {
                    while (da > 0.0f)
                        da -= NvgPi * 2;
                }
            }

            // Split arc into max 90 degree segments.
            ndivs = nvg__maxi(1, nvg__mini((int)(nvg__absf(da) / (NvgPi * 0.5f) + 0.5f), 5));
            hda = (da / ndivs) / 2.0f;
            kappa = nvg__absf(4.0f / 3.0f * (1.0f - nvg__cosf(hda)) / nvg__sinf(hda));

            if (dir == (int)NvgWinding.CounterClockwise)
                kappa = -kappa;

            nvals = 0;
            for (i = 0; i <= ndivs; i++)
            {
                a = a0 + da * (i / (float)ndivs);
                dx = nvg__cosf(a);
                dy = nvg__sinf(a);
                x = cx + dx * r;
                y = cy + dy * r;
                tanx = -dy * r * kappa;
                tany = dx * r * kappa;

                if (i == 0)
                {
                    vals[nvals++] = move;
                    vals[nvals++] = x;
                    vals[nvals++] = y;
                }
                else
                {
                    vals[nvals++] = (float)NvgCommands.BezierTo;
                    vals[nvals++] = px + ptanx;
                    vals[nvals++] = py + ptany;
                    vals[nvals++] = x - tanx;
                    vals[nvals++] = y - tany;
                    vals[nvals++] = x;
                    vals[nvals++] = y;
                }
                px = x;
                py = y;
                ptanx = tanx;
                ptany = tany;
            }

            nvg__appendCommands(ctx, vals, nvals);
        }

        public static void NvgRoundedRect(NvGcontext ctx, float x, float y, float w, float h, float r)
        {
            if (r < 0.1f)
            {
                NvgRect(ctx, x, y, w, h);
            }
            else
            {
                float rx = nvg__minf(r, nvg__absf(w) * 0.5f) * nvg__signf(w), ry = nvg__minf(r, nvg__absf(h) * 0.5f) * nvg__signf(h);
                float[] vals =
                {
                    (float)NvgCommands.MoveTo, x, y + ry,
                    (float)NvgCommands.LineTo, x, y + h - ry,
                    (float)NvgCommands.BezierTo, x, y + h - ry * (1 - NvgKappa90), x + rx * (1 - NvgKappa90), y + h, x + rx, y + h,
                    (float)NvgCommands.LineTo, x + w - rx, y + h,
                    (float)NvgCommands.BezierTo, x + w - rx * (1 - NvgKappa90), y + h, x + w, y + h - ry * (1 - NvgKappa90), x + w, y + h - ry,
                    (float)NvgCommands.LineTo, x + w, y + ry,
                    (float)NvgCommands.BezierTo, x + w, y + ry * (1 - NvgKappa90), x + w - rx * (1 - NvgKappa90), y, x + w - rx, y,
                    (float)NvgCommands.LineTo, x + rx, y,
                    (float)NvgCommands.BezierTo, x + rx * (1 - NvgKappa90), y, x, y + ry * (1 - NvgKappa90), x, y + ry,
                    (float)NvgCommands.Close
                };
                nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
            }
        }

        public static void NvgRoundedRectVarying(NvGcontext ctx, float x, float y, float w, float h, float radTopLeft, float radTopRight, float radBottomRight, float radBottomLeft)
        {
            if (radTopLeft < 0.1f && radTopRight < 0.1f && radBottomRight < 0.1f && radBottomLeft < 0.1f)
            {
                NvgRect(ctx, x, y, w, h);
                return;
            }

            float halfw = nvg__absf(w) * 0.5f;
            float halfh = nvg__absf(h) * 0.5f;
            float rxBl = nvg__minf(radBottomLeft, halfw) * nvg__signf(w), ryBl = nvg__minf(radBottomLeft, halfh) * nvg__signf(h);
            float rxBr = nvg__minf(radBottomRight, halfw) * nvg__signf(w), ryBr = nvg__minf(radBottomRight, halfh) * nvg__signf(h);
            float rxTr = nvg__minf(radTopRight, halfw) * nvg__signf(w), ryTr = nvg__minf(radTopRight, halfh) * nvg__signf(h);
            float rxTl = nvg__minf(radTopLeft, halfw) * nvg__signf(w), ryTl = nvg__minf(radTopLeft, halfh) * nvg__signf(h);
            float[] vals = {
                (float)NvgCommands.MoveTo, x, y + ryTl,
                (float)NvgCommands.LineTo, x, y + h - ryBl,
                (float)NvgCommands.BezierTo, x, y + h - ryBl*(1 - NvgKappa90), x + rxBl*(1 - NvgKappa90), y + h, x + rxBl, y + h,
                (float)NvgCommands.LineTo, x + w - rxBr, y + h,
                (float)NvgCommands.BezierTo, x + w - rxBr*(1 - NvgKappa90), y + h, x + w, y + h - ryBr*(1 - NvgKappa90), x + w, y + h - ryBr,
                (float)NvgCommands.LineTo, x + w, y + ryTr,
                (float)NvgCommands.BezierTo, x + w, y + ryTr*(1 - NvgKappa90), x + w - rxTr*(1 - NvgKappa90), y, x + w - rxTr, y,
                (float)NvgCommands.LineTo, x + rxTl, y,
                (float)NvgCommands.BezierTo, x + rxTl*(1 - NvgKappa90), y, x, y + ryTl*(1 - NvgKappa90), x, y + ryTl,
                (float)NvgCommands.Close
            };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static NvGpaint NvgLinearGradient(NvGcontext ctx,
                                                 float sx, float sy, float ex, float ey,
                                                 NvGcolor icol, NvGcolor ocol)
        {
            var p = new NvGpaint();
            float dx, dy, d;
            const float large = (float)1e5;
            //NVG_NOTUSED(ctx);
            //memset(&p, 0, sizeof(p));

            // Calculate transform aligned to the line
            dx = ex - sx;
            dy = ey - sy;
            d = (float)Math.Sqrt(dx * dx + dy * dy);
            if (d > 0.0001f)
            {
                dx /= d;
                dy /= d;
            }
            else
            {
                dx = 0;
                dy = 1;
            }

            p.Xform[0] = dy;
            p.Xform[1] = -dx;
            p.Xform[2] = dx;
            p.Xform[3] = dy;
            p.Xform[4] = sx - dx * large;
            p.Xform[5] = sy - dy * large;

            p.Extent[0] = large;
            p.Extent[1] = large + d * 0.5f;

            p.Radius = 0.0f;

            p.Feather = nvg__maxf(1.0f, d);

            p.InnerColor = icol;
            p.OuterColor = ocol;

            return p;
        }

        private static float nvg__quantize(float a, float d)
        {
            return ((int)(a / d + 0.5f)) * d;
        }

        public static NvGpaint NvgRadialGradient(NvGcontext ctx,
                                                 float cx, float cy, float inr, float outr,
                                                 NvGcolor icol, NvGcolor ocol)
        {
            var p = new NvGpaint();
            var r = (inr + outr) * 0.5f;
            var f = (outr - inr);
            //NVG_NOTUSED(ctx);
            //memset(&p, 0, sizeof(p));

            NvgTransformIdentity(p.Xform);
            p.Xform[4] = cx;
            p.Xform[5] = cy;

            p.Extent[0] = r;
            p.Extent[1] = r;

            p.Radius = r;

            p.Feather = nvg__maxf(1.0f, f);

            p.InnerColor = icol;
            p.OuterColor = ocol;

            return p;
        }

        public static void NvgEllipse(NvGcontext ctx, float cx, float cy, float rx, float ry)
        {
            float[] vals =
            {
                (float)NvgCommands.MoveTo, cx - rx, cy,
                (float)NvgCommands.BezierTo, cx - rx, cy + ry * NvgKappa90, cx - rx * NvgKappa90, cy + ry, cx, cy + ry,
                (float)NvgCommands.BezierTo, cx + rx * NvgKappa90, cy + ry, cx + rx, cy + ry * NvgKappa90, cx + rx, cy,
                (float)NvgCommands.BezierTo, cx + rx, cy - ry * NvgKappa90, cx + rx * NvgKappa90, cy - ry, cx, cy - ry,
                (float)NvgCommands.BezierTo, cx - rx * NvgKappa90, cy - ry, cx - rx, cy - ry * NvgKappa90, cx - rx, cy,
                (float)NvgCommands.Close
            };
            nvg__appendCommands(ctx, vals, NVG_COUNTOF(vals));
        }

        public static void NvgCircle(NvGcontext ctx, float cx, float cy, float r)
        {
            NvgEllipse(ctx, cx, cy, r, r);
        }

        public static int NvgTextGlyphPositions(NvGcontext ctx, float x, float y, string string_,
                                                NvGglyphPosition[] positions, int maxPositions)
        {
            var state = nvg__getState(ctx);
            var scale = nvg__getFontScale(state) * ctx.DevicePxRatio;
            var invscale = 1.0f / scale;
            FONStextIter iter = new FONStextIter(), prevIter = new FONStextIter();
            var q = new FONSquad();
            var npos = 0;

            if (state.FontId == FontStash.FONS_INVALID)
                return 0;

            //if (end == NULL)
            //	end = string + strlen(string);

            //if (string_ == end)
            //	return 0;

            FontStash.fonsSetSize(ref ctx.Fs, state.FontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.Fs, state.LetterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.Fs, state.FontBlur * scale);
            FontStash.fonsSetAlign(ctx.Fs, (FONSalign)state.TextAlign);
            FontStash.fonsSetFont(ref ctx.Fs, state.FontId);

            FontStash.fonsTextIterInit(ctx.Fs, ref iter, x * scale, y * scale, string_);
            prevIter = iter;
            while (FontStash.fonsTextIterNext(ctx.Fs, ref iter, ref q) != 0)
            {
                if (iter.prevGlyphIndex < 0 && nvg__allocTextAtlas(ctx) > 0)
                { // can not retrieve glyph?
                    iter = prevIter;
                    FontStash.fonsTextIterNext(ctx.Fs, ref iter, ref q); // try again
                }
                prevIter = iter;
                positions[npos].Str = iter.iStr;
                positions[npos].X = iter.x * invscale;
                positions[npos].Minx = nvg__minf(iter.x, q.x0) * invscale;
                positions[npos].Maxx = nvg__maxf(iter.nextx, q.x1) * invscale;
                npos++;
                if (npos >= maxPositions)
                    break;
            }

            return npos;
        }

        public static void NvgTextBox(NvGcontext ctx, float x, float y, float breakRowWidth, string string_)
        {
            var state = nvg__getState(ctx);
            var rows = new NvGtextRow[2];
            int nrows = 0, i;
            var oldAlign = state.TextAlign;
            var haling = state.TextAlign & ((int)NvgAlign.Left | (int)NvgAlign.Center | (int)NvgAlign.Right);
            var valign = state.TextAlign & ((int)NvgAlign.Top | (int)NvgAlign.Middle | (int)NvgAlign.Bottom | (int)NvgAlign.Baseline);
            float lineh = 0;
            float fnull = 0;

            if (state.FontId == FontStash.FONS_INVALID)
                return;

            NvgTextMetrics(ctx, ref fnull, ref fnull, ref lineh);

            state.TextAlign = (int)NvgAlign.Left | valign;

            while ((nrows = NvgTextBreakLines(ctx, string_, breakRowWidth, rows, 2)) > 0)
            {
                for (i = 0; i < nrows; i++)
                {
                    string str;
                    var row = rows[i];
                    if ((haling & (int)NvgAlign.Left) != 0)
                    {
                        str = string_.Substring(row.Start, row.End - row.Start);
                        NvgText(ctx, x, y, str);
                    }
                    else if ((haling & (int)NvgAlign.Center) != 0)
                    {
                        str = string_.Substring(row.Start, row.End - row.Start);
                        NvgText(ctx, x + breakRowWidth * 0.5f - row.Width * 0.5f, y, str);
                    }
                    else if ((haling & (int)NvgAlign.Right) != 0)
                    {
                        str = string_.Substring(row.Start, row.End - row.Start);
                        NvgText(ctx, x + breakRowWidth - row.Width, y, str);
                    }
                    y += lineh * state.LineHeight;
                }
                string_ = string_.Substring(rows[nrows - 1].Next);

                if (string_.Length == 1)
                    string_ = "";
            }

            state.TextAlign = oldAlign;
        }

        public static void NvgTextBoxBounds(NvGcontext ctx, float x, float y, float breakRowWidth, string string_, float[] bounds)
        {
            var state = nvg__getState(ctx);
            var rows = new NvGtextRow[2];
            var scale = nvg__getFontScale(state) * ctx.DevicePxRatio;
            var invscale = 1.0f / scale;
            int nrows = 0, i;
            var oldAlign = state.TextAlign;
            var haling = state.TextAlign & ((int)NvgAlign.Left | (int)NvgAlign.Center | (int)NvgAlign.Right);
            var valign = state.TextAlign & ((int)NvgAlign.Top | (int)NvgAlign.Middle | (int)NvgAlign.Bottom | (int)NvgAlign.Baseline);
            float lineh = 0, rminy = 0, rmaxy = 0;
            float minx, miny, maxx, maxy;
            float fnull = 0;

            if (state.FontId == FontStash.FONS_INVALID)
            {
                if (bounds != null)
                    bounds[0] = bounds[1] = bounds[2] = bounds[3] = 0.0f;
                return;
            }

            NvgTextMetrics(ctx, ref fnull, ref fnull, ref lineh);

            state.TextAlign = (int)NvgAlign.Left | valign;

            minx = maxx = x;
            miny = maxy = y;

            FontStash.fonsSetSize(ref ctx.Fs, state.FontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.Fs, state.LetterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.Fs, state.FontBlur * scale);
            FontStash.fonsSetAlign(ctx.Fs, (FONSalign)state.TextAlign);
            FontStash.fonsSetFont(ref ctx.Fs, state.FontId);
            FontStash.fonsLineBounds(ctx.Fs, 0, ref rminy, ref rmaxy);
            rminy *= invscale;
            rmaxy *= invscale;

            while ((nrows = NvgTextBreakLines(ctx, string_, breakRowWidth, rows, 2)) > 0)
            {
                for (i = 0; i < nrows; i++)
                {
                    var row = rows[i];
                    float rminx, rmaxx, dx = 0;
                    // Horizontal bounds
                    if ((haling & (int)NvgAlign.Left) != 0)
                        dx = 0;
                    else if ((haling & (int)NvgAlign.Center) != 0)
                        dx = breakRowWidth * 0.5f - row.Width * 0.5f;
                    else if ((haling & (int)NvgAlign.Right) != 0)
                        dx = breakRowWidth - row.Width;
                    rminx = x + row.Minx + dx;
                    rmaxx = x + row.Maxx + dx;
                    minx = nvg__minf(minx, rminx);
                    maxx = nvg__maxf(maxx, rmaxx);
                    // Vertical bounds.
                    miny = nvg__minf(miny, y + rminy);
                    maxy = nvg__maxf(maxy, y + rmaxy);

                    y += lineh * state.LineHeight;
                }
                string_ = string_.Substring(rows[nrows - 1].Next);

                if (string_.Length == 1)
                    string_ = "";
            }

            state.TextAlign = oldAlign;

            if (bounds != null)
            {
                bounds[0] = minx;
                bounds[1] = miny;
                bounds[2] = maxx;
                bounds[3] = maxy;
            }
        }

        public static float NvgTextBounds(NvGcontext ctx, float x, float y, string string_, float[] bounds)
        {
            var state = nvg__getState(ctx);
            var scale = nvg__getFontScale(state) * ctx.DevicePxRatio;
            var invscale = 1.0f / scale;
            float width;

            if (state.FontId == FontStash.FONS_INVALID)
                return 0;

            FontStash.fonsSetSize(ref ctx.Fs, state.FontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.Fs, state.LetterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.Fs, state.FontBlur * scale);
            FontStash.fonsSetAlign(ctx.Fs, (FONSalign)state.TextAlign);
            FontStash.fonsSetFont(ref ctx.Fs, state.FontId);

            width = FontStash.fonsTextBounds(ref ctx.Fs, x * scale, y * scale, string_, bounds);
            if (bounds != null)
            {
                // Use line bounds for height.
                FontStash.fonsLineBounds(ctx.Fs, y * scale, ref bounds[1], ref bounds[3]);
                bounds[0] *= invscale;
                bounds[1] *= invscale;
                bounds[2] *= invscale;
                bounds[3] *= invscale;
            }
            return width * invscale;
        }

        public static void NvgTextMetrics(NvGcontext ctx, ref float ascender, ref float descender, ref float lineh)
        {
            var state = nvg__getState(ctx);
            var scale = nvg__getFontScale(state) * ctx.DevicePxRatio;
            var invscale = 1.0f / scale;

            if (state.FontId == FontStash.FONS_INVALID)
                return;

            FontStash.fonsSetSize(ref ctx.Fs, state.FontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.Fs, state.LetterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.Fs, state.FontBlur * scale);
            FontStash.fonsSetAlign(ctx.Fs, (FONSalign)state.TextAlign);
            FontStash.fonsSetFont(ref ctx.Fs, state.FontId);

            FontStash.fonsVertMetrics(ref ctx.Fs, ref ascender, ref descender, ref lineh);
            ascender *= invscale;
            descender *= invscale;
            lineh *= invscale;
        }

        public static int NvgTextBreakLines(NvGcontext ctx, string string_,
                                            float breakRowWidth, NvGtextRow[] rows, int maxRows)
        {
            var state = nvg__getState(ctx);
            var scale = nvg__getFontScale(state) * ctx.DevicePxRatio;
            var invscale = 1.0f / scale;
            FONStextIter iter = new FONStextIter(), prevIter = new FONStextIter();
            var q = new FONSquad();
            var nrows = 0;
            float rowStartX = 0;
            float rowWidth = 0;
            float rowMinX = 0;
            float rowMaxX = 0;
            var rowStart = -1;
            var rowEnd = -1;
            var wordStart = -1;
            float wordStartX = 0;
            float wordMinX = 0;
            var breakEnd = -1;
            float breakWidth = 0;
            float breakMaxX = 0;
            int type = (int)NvgCodepointType.Space, ptype = (int)NvgCodepointType.Space;
            uint pcodepoint = 0;

            if (string_ == null)
                return 0;
            var end = string_.Length - 1;

            if (maxRows == 0)
                return 0;
            if (state.FontId == FontStash.FONS_INVALID)
                return 0;

            FontStash.fonsSetSize(ref ctx.Fs, state.FontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.Fs, state.LetterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.Fs, state.FontBlur * scale);
            FontStash.fonsSetAlign(ctx.Fs, (FONSalign)state.TextAlign);
            FontStash.fonsSetFont(ref ctx.Fs, state.FontId);

            breakRowWidth *= scale;

            FontStash.fonsTextIterInit(ctx.Fs, ref iter, 0, 0, string_);
            prevIter = iter;
            while (FontStash.fonsTextIterNext(ctx.Fs, ref iter, ref q) != 0)
            {
                // can not retrieve glyph?
                if (iter.prevGlyphIndex < 0 && nvg__allocTextAtlas(ctx) > 0)
                {
                    iter = prevIter;
                    FontStash.fonsTextIterNext(ctx.Fs, ref iter, ref q); // try again
                }
                prevIter = iter;
                switch (iter.codepoint)
                {
                    case 9:         // \t
                    case 11:        // \v
                    case 12:        // \f
                    case 32:        // space
                    case 0x00a0:    // NBSP
                        type = (int)NvgCodepointType.Space;
                        break;
                    case 10:        // \n
                        type = pcodepoint == 13 ? (int)NvgCodepointType.Space : (int)NvgCodepointType.Newline;
                        break;
                    case 13:        // \r
                        type = pcodepoint == 10 ? (int)NvgCodepointType.Space : (int)NvgCodepointType.Newline;
                        break;
                    case 0x0085:    // NEL
                        type = (int)NvgCodepointType.Newline;
                        break;
                    default:
                        type = (int)NvgCodepointType.Char;
                        break;
                }

                if (type == (int)NvgCodepointType.Newline)
                {
                    // Always handle new lines.
                    rows[nrows].Start = rowStart >= 0 ? rowStart : iter.iStr;
                    var rs = string_.Substring(rows[nrows].Start);

                    rows[nrows].End = rowEnd >= 0 ? rowEnd : iter.iStr;
                    var re = string_.Substring(rows[nrows].End);

                    rows[nrows].Width = rowWidth * invscale;
                    rows[nrows].Minx = rowMinX * invscale;
                    rows[nrows].Maxx = rowMaxX * invscale;
                    rows[nrows].Next = iter.iNext;
                    var inx = string_.Substring(rows[nrows].Next);

                    nrows++;
                    if (nrows >= maxRows)
                        return nrows;
                    // Set null break point
                    breakEnd = rowStart;
                    breakWidth = 0.0f;
                    breakMaxX = 0.0f;
                    // Indicate to skip the white space at the beginning of the row.
                    rowStart = -1;
                    rowEnd = -1;
                    rowWidth = 0;
                    rowMinX = rowMaxX = 0;
                }
                else
                {
                    if (rowStart < 0)
                    {
                        // Skip white space until the beginning of the line
                        if (type == (int)NvgCodepointType.Char)
                        {
                            // The current char is the row so far
                            rowStartX = iter.x;
                            rowStart = iter.iStr;
                            var rs = string_.Substring(rowStart);

                            rowEnd = iter.iNext;
                            var re = string_.Substring(rowEnd);

                            rowWidth = iter.nextx - rowStartX; // q.x1 - rowStartX;
                            rowMinX = q.x0 - rowStartX;
                            rowMaxX = q.x1 - rowStartX;
                            wordStart = iter.iStr;
                            var ws = string_.Substring(wordStart);

                            wordStartX = iter.x;
                            wordMinX = q.x0 - rowStartX;
                            // Set null break point
                            breakEnd = rowStart;
                            breakWidth = 0.0f;
                            breakMaxX = 0.0f;
                        }
                    }
                    else
                    {
                        var nextWidth = iter.nextx - rowStartX;

                        // track last non-white space character
                        if (type == (int)NvgCodepointType.Char)
                        {
                            rowEnd = iter.iNext;
                            rowWidth = iter.nextx - rowStartX;
                            rowMaxX = q.x1 - rowStartX;
                        }
                        // track last end of a word
                        if (ptype == (int)NvgCodepointType.Char && type == (int)NvgCodepointType.Space)
                        {
                            breakEnd = iter.iStr;
                            breakWidth = rowWidth;
                            breakMaxX = rowMaxX;
                        }
                        // track last beginning of a word
                        if (ptype == (int)NvgCodepointType.Space && type == (int)NvgCodepointType.Char)
                        {
                            wordStart = iter.iStr;
                            wordStartX = iter.x;
                            wordMinX = q.x0 - rowStartX;
                        }

                        // Break to new line when a character is beyond break width.
                        if (type == (int)NvgCodepointType.Char && nextWidth > breakRowWidth)
                        {
                            // The run length is too long, need to break to new line.
                            if (breakEnd == rowStart)
                            {
                                // The current word is longer than the row length, just break it from here.
                                rows[nrows].Start = rowStart;
                                rows[nrows].End = iter.iStr;
                                rows[nrows].Width = rowWidth * invscale;
                                rows[nrows].Minx = rowMinX * invscale;
                                rows[nrows].Maxx = rowMaxX * invscale;
                                rows[nrows].Next = iter.iStr;
                                nrows++;
                                if (nrows >= maxRows)
                                    return nrows;
                                rowStartX = iter.x;
                                rowStart = iter.iStr;
                                rowEnd = iter.iNext;
                                rowWidth = iter.nextx - rowStartX;
                                rowMinX = q.x0 - rowStartX;
                                rowMaxX = q.x1 - rowStartX;
                                wordStart = iter.iStr;
                                wordStartX = iter.x;
                                wordMinX = q.x0 - rowStartX;
                            }
                            else
                            {
                                // Break the line from the end of the last word, and start new line from the beginning of the new.
                                rows[nrows].Start = rowStart;
                                rows[nrows].End = breakEnd;
                                rows[nrows].Width = breakWidth * invscale;
                                rows[nrows].Minx = rowMinX * invscale;
                                rows[nrows].Maxx = breakMaxX * invscale;
                                rows[nrows].Next = wordStart;
                                nrows++;
                                if (nrows >= maxRows)
                                    return nrows;
                                rowStartX = wordStartX;
                                rowStart = wordStart;
                                rowEnd = iter.iNext;
                                rowWidth = iter.nextx - rowStartX;
                                rowMinX = wordMinX;
                                rowMaxX = q.x1 - rowStartX;
                                // No change to the word start
                            }
                            // Set null break point
                            breakEnd = rowStart;
                            breakWidth = 0.0f;
                            breakMaxX = 0.0f;
                        }
                    }
                }

                pcodepoint = iter.codepoint;
                ptype = type;
            }

            // Break the line from the end of the last word, and start new line from the beginning of the new.
            if (rowStart >= 0)
            {
                rows[nrows].Start = rowStart;
                rows[nrows].End = rowEnd;
                rows[nrows].Width = rowWidth * invscale;
                rows[nrows].Minx = rowMinX * invscale;
                rows[nrows].Maxx = rowMaxX * invscale;
                rows[nrows].Next = end;
                nrows++;
            }

            return nrows;
        }

        public static void NvgTextLineHeight(NvGcontext ctx, float lineHeight)
        {
            var state = nvg__getState(ctx);
            state.LineHeight = lineHeight;
        }

        public static void NvgTextAlign(NvGcontext ctx, int align)
        {
            var state = nvg__getState(ctx);
            state.TextAlign = align;
        }

        private static float nvg__getFontScale(NvGstate state)
        {
            return nvg__minf(nvg__quantize(nvg__getAverageScale(state.Xform), 0.01f), 4.0f);
        }

        public static void NvgImageSize(NvGcontext ctx, int image, ref int w, ref int h)
        {
            ctx.Params.RenderGetTextureSize(ctx.Params.UserPtr, image, ref w, ref h);
        }

        public static NvGpaint NvgImagePattern(NvGcontext ctx,
                                               float cx, float cy, float w, float h, float angle,
                                               int image, float alpha)
        {
            var p = new NvGpaint();
            //NVG_NOTUSED(ctx);
            //memset(&p, 0, sizeof(p));

            NvgTransformRotate(p.Xform, angle);
            p.Xform[4] = cx;
            p.Xform[5] = cy;

            p.Extent[0] = w;
            p.Extent[1] = h;

            p.Image = image;

            p.InnerColor = p.OuterColor = NvgRgbAf(1, 1, 1, alpha);

            return p;
        }

        private static int nvg__allocTextAtlas(NvGcontext ctx)
        {
            int iw = 0, ih = 0;
            nvg__flushTextTexture(ctx);
            if (ctx.FontImageIdx >= NvgMaxFontimages - 1)
                return 0;
            // if next fontImage already have a texture
            if (ctx.FontImages[ctx.FontImageIdx + 1] != 0)
                NvgImageSize(ctx, ctx.FontImages[ctx.FontImageIdx + 1], ref iw, ref ih);
            else
            { // calculate the new font image size and create it.
                NvgImageSize(ctx, ctx.FontImages[ctx.FontImageIdx], ref iw, ref ih);
                if (iw > ih)
                    ih *= 2;
                else
                    iw *= 2;
                if (iw > NvgMaxFontimageSize || ih > NvgMaxFontimageSize)
                    iw = ih = NvgMaxFontimageSize;
                ctx.FontImages[ctx.FontImageIdx + 1] = ctx.Params.RenderCreateTexture(ctx.Params.UserPtr,
                    (int)NvgTexture.Alpha, iw, ih, 0, null);
            }
            ++ctx.FontImageIdx;
            FontStash.fonsResetAtlas(ctx.Fs, iw, ih);
            return 1;
        }

        public static float NvgText(NvGcontext ctx, float x, float y, string string_)
        {
            var state = nvg__getState(ctx);
            FONStextIter iter = new FONStextIter(), prevIter = new FONStextIter();
            var q = new FONSquad();
            NvGvertex[] verts;
            var scale = nvg__getFontScale(state) * ctx.DevicePxRatio;
            var invscale = 1.0f / scale;
            var cverts = 0;
            var nverts = 0;

            var end = string_.Length;

            if (state.FontId == FontStash.FONS_INVALID)
                return x;

            FontStash.fonsSetSize(ref ctx.Fs, state.FontSize * scale);
            FontStash.fonsSetSpacing(ref ctx.Fs, state.LetterSpacing * scale);
            FontStash.fonsSetBlur(ref ctx.Fs, state.FontBlur * scale);
            FontStash.fonsSetAlign(ctx.Fs, (FONSalign)state.TextAlign);
            FontStash.fonsSetFont(ref ctx.Fs, state.FontId);

            cverts = nvg__maxi(2, end) * 6; // conservative estimate.
            verts = nvg__allocTempVerts(ctx, cverts);
            if (verts == null)
                return x;

            FontStash.fonsTextIterInit(ctx.Fs, ref iter, x * scale, y * scale, string_);
            prevIter = iter;
            while (FontStash.fonsTextIterNext(ctx.Fs, ref iter, ref q) != 0)
            {
                var c = new float[4 * 2];
                if (iter.prevGlyphIndex == -1)
                { // can not retrieve glyph?
                    if (nvg__allocTextAtlas(ctx) == 0)
                        break; // no memory :(
                    if (nverts != 0)
                    {
                        nvg__renderText(ctx, verts, nverts);
                        nverts = 0;
                    }
                    iter = prevIter;
                    FontStash.fonsTextIterNext(ctx.Fs, ref iter, ref q); // try again
                    if (iter.prevGlyphIndex == -1) // still can not find glyph?
                        break;
                }
                prevIter = iter;
                // Transform corners.
                NvgTransformPoint(ref c[0], ref c[1], state.Xform, q.x0 * invscale, q.y0 * invscale);
                NvgTransformPoint(ref c[2], ref c[3], state.Xform, q.x1 * invscale, q.y0 * invscale);
                NvgTransformPoint(ref c[4], ref c[5], state.Xform, q.x1 * invscale, q.y1 * invscale);
                NvgTransformPoint(ref c[6], ref c[7], state.Xform, q.x0 * invscale, q.y1 * invscale);
                // Create triangles
                if (nverts + 6 <= cverts)
                {
                    nvg__vset(ref verts[nverts], c[0], c[1], q.s0, q.t0);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[4], c[5], q.s1, q.t1);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[2], c[3], q.s1, q.t0);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[0], c[1], q.s0, q.t0);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[6], c[7], q.s0, q.t1);
                    nverts++;
                    nvg__vset(ref verts[nverts], c[4], c[5], q.s1, q.t1);
                    nverts++;
                }
            }

            //ctx.cache.verts = verts;

            // TODO: add back-end bit to do this just once per frame.
            nvg__flushTextTexture(ctx);

            nvg__renderText(ctx, verts, nverts);

            return iter.x;
        }

        private static void nvg__renderText(NvGcontext ctx, NvGvertex[] verts, int nverts)
        {
            var state = nvg__getState(ctx);
            // last change
            var paint = state.Fill.Clone();

            // Render triangles.
            paint.Image = ctx.FontImages[ctx.FontImageIdx];

            // Apply global alpha
            paint.InnerColor.A *= state.Alpha;
            paint.OuterColor.A *= state.Alpha;

            ctx.Params.RenderTriangles(ctx.Params.UserPtr, ref paint, ref state.Scissor, verts, nverts);

            ctx.DrawCallCount++;
            ctx.TextTriCount += nverts / 3;
        }

        private static void nvg__flushTextTexture(NvGcontext ctx)
        {
            var dirty = new int[4];

            if (FontStash.fonsValidateTexture(ctx.Fs, dirty) != 0)
            {
                var fontImage = ctx.FontImages[ctx.FontImageIdx];
                // Update texture
                if (fontImage != 0)
                {
                    int iw = 0, ih = 0;
                    var data = FontStash.fonsGetTextureData(ctx.Fs, ref iw, ref ih);
                    var x = dirty[0];
                    var y = dirty[1];
                    var w = dirty[2] - dirty[0];
                    var h = dirty[3] - dirty[1];
                    ctx.Params.RenderUpdateTexture(ctx.Params.UserPtr, fontImage, x, y, w, h, data);
                }
            }
        }

        public static void NvgGlobalAlpha(NvGcontext ctx, float alpha)
        {
            var state = nvg__getState(ctx);
            state.Alpha = alpha;
        }

        private static void NvgTransformIdentity(float[] t)
        {
            t[0] = 1.0f;
            t[1] = 0.0f;
            t[2] = 0.0f;
            t[3] = 1.0f;
            t[4] = 0.0f;
            t[5] = 0.0f;
        }

        private static float nvg__hue(float h, float m1, float m2)
        {
            if (h < 0)
                h += 1;
            if (h > 1)
                h -= 1;
            if (h < 1.0f / 6.0f)
                return m1 + (m2 - m1) * h * 6.0f;
            if (h < 3.0f / 6.0f)
                return m2;
            if (h < 4.0f / 6.0f)
                return m1 + (m2 - m1) * (2.0f / 3.0f - h) * 6.0f;
            return m1;
        }

        public static NvGcolor NvgHsla(float h, float s, float l, byte a)
        {
            float m1, m2;
            NvGcolor col;
            h = nvg__modf(h, 1.0f);
            if (h < 0.0f)
                h += 1.0f;
            s = nvg__clampf(s, 0.0f, 1.0f);
            l = nvg__clampf(l, 0.0f, 1.0f);
            m2 = l <= 0.5f ? (l * (1 + s)) : (l + s - l * s);
            m1 = 2 * l - m2;
            col.R = nvg__clampf(nvg__hue(h + 1.0f / 3.0f, m1, m2), 0.0f, 1.0f);
            col.G = nvg__clampf(nvg__hue(h, m1, m2), 0.0f, 1.0f);
            col.B = nvg__clampf(nvg__hue(h - 1.0f / 3.0f, m1, m2), 0.0f, 1.0f);
            col.A = a / 255.0f;
            return col;
        }

        public static int NvgAddFallbackFontId(NvGcontext ctx, int baseFont, int fallbackFont)
        {
            if (baseFont == -1 || fallbackFont == -1)
                return 0;
            return FontStash.fonsAddFallbackFont(ctx.Fs, baseFont, fallbackFont);
        }

        public static void NvgBeginFrame(NvGcontext ctx, int windowWidth, int windowHeight, float devicePixelRatio)
        {
            ctx.Nstates = 0;
            NvgSave(ctx);
            NvgReset(ctx);

            nvg__setDevicePixelRatio(ref ctx, devicePixelRatio);

            ctx.Params.RenderViewport(ctx.Params.UserPtr, windowWidth, windowHeight, devicePixelRatio);

            ctx.DrawCallCount = 0;
            ctx.FillTriCount = 0;
            ctx.StrokeTriCount = 0;
            ctx.TextTriCount = 0;
        }

        /// <summary>
        /// Create font from *.ttf <see cref="fileName"/>.
        /// </summary>
        /// <returns>The create font id.</returns>
        /// <param name="ctx">NanoVG context.</param>
        /// <param name="internalFontName">Internal font name.</param>
        /// <param name="fileName">File name of *.ttf font file (can include a path).</param>
        public static int NvgCreateFont(NvGcontext ctx, string internalFontName, string fileName)
        {
            return FontStash.fonsAddFont(ctx.Fs, internalFontName, fileName);
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }

        private static int NvgCreateImageRgba(ref NvGcontext ctx, int w, int h, int imageFlags, byte[] data)
        {
            return ctx.Params.RenderCreateTexture(ctx.Params.UserPtr, (int)NvgTexture.Rgba, w, h, imageFlags, data);
        }

        private static int NvgCreateImageRgba(ref NvGcontext ctx, int w, int h, int imageFlags, Bitmap bmp)
        {
            return ctx.Params.RenderCreateTexture2(ctx.Params.UserPtr, (int)NvgTexture.Rgba, w, h, imageFlags, bmp);
        }

        /// <summary>
        /// Convert a bitmap to a byte array
        /// </summary>
        /// <param name="bitmap">image to convert</param>
        /// <returns>image as bytes</returns>
        private static byte[] ConvertBitmap(Bitmap bitmap)
        {
            //Code excerpted from Microsoft Robotics Studio v1.5
            BitmapData raw = null;  //used to get attributes of the image
            byte[] rawImage = null; //the image as a byte[]

            try
            {
                //Freeze the image in memory
                raw = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb
                );

                var size = raw.Height * raw.Stride;
                rawImage = new byte[size];

                //Copy the image into the byte[]
                Marshal.Copy(raw.Scan0, rawImage, 0, size);
            }
            finally
            {
                if (raw != null)
                {
                    //Unfreeze the memory for the image
                    bitmap.UnlockBits(raw);
                }
            }
            return rawImage;
        }

        public static int NvgCreateImage(ref NvGcontext ctx, string filename, int imageFlags)
        {
            //int w, h, n;
            var image = 0;
            //byte[] img;
            Bitmap bmp = null;
            //stbi_set_unpremultiply_on_load(1);
            //stbi_convert_iphone_png_to_rgb(1);
            //img = stbi_load(filename, &w, &h, &n, 4);
            //Image loadedImg = Image.FromFile(filename);
            //img = ImageToByteArray(loadedImg);
            try
            {
                bmp = new Bitmap(filename);
            }
            catch
            {
                //if (img == null)
                {
                    //		printf("Failed to load %s - %s\n", filename, stbi_failure_reason());
                    return 0;
                }
            }
            image = NvgCreateImageRgba(ref ctx, bmp.Width, bmp.Height, imageFlags, bmp);
            //stbi_image_free(img);
            return image;
        }
    }

    #region Auxiliary-classes-structs

    public class NvGpoint
    {
        public float X, Y;
        public float Dx, Dy;
        public float Len;
        public float Dmx, Dmy;
        public byte Flags;

        public override string ToString()
        {
            return string.Format("[NVGpoint]x={0}, y={1}, dx={2}, dy={3}, len={4}, dmx={5}, dmy={6}",
                X, Y, Dx, Dy, Len);
        }

        public NvGpoint Clone()
        {
            var newpoint = new NvGpoint();

            newpoint.X = X;
            newpoint.Y = Y;
            newpoint.Dx = Dx;
            newpoint.Dy = Dy;
            newpoint.Len = Len;
            newpoint.Dmx = Dmx;
            newpoint.Dmy = Dmy;
            newpoint.Flags = Flags;

            return newpoint;
        }
    }

    public struct NvGvertex
    {
        public float X;
        public float Y;
        public float U;
        public float V;

        public override string ToString()
        {
            return $"[NVGvertex] x={X} y={Y} u={U} v={V}";
        }
    }

    public class NvGpath
    {
        public int First;
        public int Count;
        public byte Closed;
        public int Nbevel;
        public NvGvertex[] Fill;
        public int Ifill;
        public int Nfill;
        public NvGvertex[] Stroke;
        public int Istroke;
        public int Nstroke;
        public int Winding;
        public int Convex;
    }

    public class NvGpathCache
    {
        public NvGpoint[] Points;
        public int Npoints;
        public int Cpoints;
        public NvGpath[] Paths;
        public int Npaths;
        public int Cpaths;
        public NvGvertex[] Verts;
        public int Nverts;
        public int Cverts;
        //[4];
        public float[] Bounds;

        public NvGpathCache()
        {
            Bounds = new float[4];
        }
    }

    public struct NvGparams
    {
        public object UserPtr;
        public int EdgeAntiAlias;
        public RenderCreateHandler RenderCreate;

        public RenderCreateTextureHandler RenderCreateTexture;
        public RenderCreateTextureHandler2 RenderCreateTexture2;
        public RenderViewportHandler RenderViewport;
        public RenderFlushHandler RenderFlush;
        public RenderFillHandler RenderFill;
        public RenderStrokeHandler RenderStroke;
        public RenderTrianglesHandler RenderTriangles;
        public RenderUpdateTextureHandler RenderUpdateTexture;
        public RenderGetTextureSizeHandler RenderGetTextureSize;
        public RenderDeleteTexture RenderDeleteTexture;
        public RenderCancel RenderCancel;
        public RenderDelete RenderDelete;
    }

    public class NvGcontext
    {
        public NvGparams Params;
        public float[] Commands;
        public int Ccommands;
        public int Ncommands;
        public float Commandx, Commandy;
        //[NVG_MAX_STATES];
        public NvGstate[] States;
        public int Nstates;
        public NvGpathCache Cache;
        public float TessTol;
        public float DistTol;
        public float FringeWidth;
        public float DevicePxRatio;
        public FONScontext Fs;
        //[NVG_MAX_FONTIMAGES];
        public int[] FontImages;
        public int FontImageIdx;
        public int DrawCallCount;
        public int FillTriCount;
        public int StrokeTriCount;
        public int TextTriCount;

        public NvGcontext()
        {
            States = new NvGstate[NanoVg.NvgMaxStates];
            for (var cont = 0; cont < States.Length; cont++)
                States[cont] = new NvGstate();
            FontImages = new int[NanoVg.NvgMaxFontimages];
        }
    }

    public struct NvGcompositeOperationState
    {
        public int SrcRgb;
        public int DstRgb;
        public int SrcAlpha;
        public int DstAlpha;
    }

    //[StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Rgba
    {
        //[FieldOffset(0)]
        public float R;
        //[FieldOffset(4)]
        public float G;
        //[FieldOffset(8)]
        public float B;
        //[FieldOffset(16)]
        public float A;
    }

    public struct NvGcolor
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public override string ToString()
        {
            return $"[NVGcolor: r={R}, g={G}, b={B}, a={A}]";
        }
    }

    public class NvGscissor
    {
        //[6];
        public float[] Xform;
        //[2];
        public float[] Extent;

        public NvGscissor()
        {
            Xform = new float[6];
            Extent = new float[2];
        }

        public NvGscissor Clone()
        {
            var newScissor = new NvGscissor();

            Array.Copy(Xform, newScissor.Xform, Xform.Length);
            Array.Copy(Extent, newScissor.Extent, Extent.Length);

            return newScissor;
        }
    }

    public class NvGstate
    {
        public NvGcompositeOperationState CompositeOperation;
        public NvGpaint Fill;
        public NvGpaint Stroke;
        public float StrokeWidth;
        public float MiterLimit;
        public int LineJoin;
        public int LineCap;
        public float Alpha;
        //[6];
        public float[] Xform;
        public NvGscissor Scissor;
        public float FontSize;
        public float LetterSpacing;
        public float LineHeight;
        public float FontBlur;
        public int TextAlign;
        public int FontId;

        public NvGstate()
        {
            Xform = new float[6];
            Scissor = new NvGscissor();
            Fill = new NvGpaint();
            Stroke = new NvGpaint();
        }

        public NvGstate Clone()
        {
            var newState = new NvGstate();
            newState.CompositeOperation = CompositeOperation;
            newState.Fill = Fill.Clone();
            newState.Stroke = Stroke.Clone();
            newState.StrokeWidth = StrokeWidth;
            newState.MiterLimit = MiterLimit;
            newState.LineJoin = LineJoin;
            newState.LineCap = LineCap;
            newState.Alpha = Alpha;

            Array.Copy(Xform, newState.Xform, Xform.Length);

            newState.Scissor = Scissor.Clone();
            newState.FontSize = FontSize;
            newState.LetterSpacing = LetterSpacing;
            newState.LineHeight = LineHeight;
            newState.FontBlur = FontBlur;
            newState.TextAlign = TextAlign;
            newState.FontId = FontId;

            return newState;
        }
    }

    public class NvGpaint
    {
        //[6];
        public float[] Xform;
        //[2];
        public float[] Extent;
        public float Radius;
        public float Feather;
        public NvGcolor InnerColor;
        public NvGcolor OuterColor;
        public int Image;

        public NvGpaint()
        {
            Xform = new float[6];
            Extent = new float[2];
        }

        public NvGpaint Clone()
        {
            var newPaint = new NvGpaint();

            Array.Copy(Xform, newPaint.Xform, Xform.Length);
            Array.Copy(Extent, newPaint.Extent, Extent.Length);
            newPaint.Radius = Radius;
            newPaint.Feather = Feather;
            newPaint.InnerColor = InnerColor;
            newPaint.OuterColor = OuterColor;
            newPaint.Image = Image;

            return newPaint;
        }
    }

    public struct NvGtextRow
    {
        public int Start;
        // Pointer to the input text where the row starts.
        public int End;
        // Pointer to the input text where the row ends (one past the last character).
        public int Next;
        // Pointer to the beginning of the next row.
        public float Width;
        // Logical width of the row.
        public float Minx, Maxx;
        // Actual bounds of the row. Logical with and bounds can differ because of kerning and some parts over extending.
    }

    public struct NvGglyphPosition
    {
        public int Str;
        // Position of the glyph in the input string.
        public float X;
        // The x-coordinate of the logical glyph position.
        public float Minx, Maxx;
        // The bounds of the glyph shape.
    }
    #endregion Auxiliary-classes-structs
}

