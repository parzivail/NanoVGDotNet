
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

using System;
using NanoVGDotNet;

namespace NanoVGDotNet
{
	public static class PerfGraph
	{
		public const int GraphHistoryCount = 100;

		static int _style;
		static string _name;
		static float[] _values;
		static int _head;

		public static void InitGraph(int style, string name)
		{
			PerfGraph._style = style;
			PerfGraph._name = name;
			_values = new float[GraphHistoryCount];
			_head = 0;
		}

		public static void UpdateGraph(float frameTime)
		{
			_head = (_head + 1) % GraphHistoryCount;
			_values[_head] = frameTime;
		}

		public static float GetGraphAverage()
		{
			int i;
			float avg = 0;
			for (i = 0; i < GraphHistoryCount; i++)
			{
				avg += _values[i];
			}
			return avg / (float)GraphHistoryCount;
		}

		public static void RenderGraph(NvGcontext vg, float x, float y)
		{
			int i;
			float avg, w, h;
			string str;

			avg = GetGraphAverage();

			w = 200;
			h = 35;

			NanoVg.NvgBeginPath(vg);
			NanoVg.NvgRect(vg, x, y, w, h);
			NanoVg.NvgFillColor(vg, NanoVg.NvgRgba(0, 0, 0, 128));
			NanoVg.NvgFill(vg);

			NanoVg.NvgBeginPath(vg);
			NanoVg.NvgMoveTo(vg, x, y + h);
			if (_style == (int)GraphRenderStyle.Fps)
			{
				for (i = 0; i < GraphHistoryCount; i++)
				{
					var v = 1.0f / (0.00001f + _values[(_head + i) % GraphHistoryCount]);
					float vx, vy;
					if (v > 80.0f)
						v = 80.0f;
					vx = x + ((float)i / (GraphHistoryCount - 1)) * w;
					vy = y + h - ((v / 80.0f) * h);
					NanoVg.NvgLineTo(vg, vx, vy);
				}
			}
			else if (_style == (int)GraphRenderStyle.Percent)
			{
				for (i = 0; i < GraphHistoryCount; i++)
				{
					var v = _values[(_head + i) % GraphHistoryCount] * 1.0f;
					float vx, vy;
					if (v > 100.0f)
						v = 100.0f;
					vx = x + ((float)i / (GraphHistoryCount - 1)) * w;
					vy = y + h - ((v / 100.0f) * h);
					NanoVg.NvgLineTo(vg, vx, vy);
				}
			}
			else
			{
				for (i = 0; i < GraphHistoryCount; i++)
				{
					var v = _values[(_head + i) % GraphHistoryCount] * 1000.0f;
					float vx, vy;
					if (v > 20.0f)
						v = 20.0f;
					vx = x + ((float)i / (GraphHistoryCount - 1)) * w;
					vy = y + h - ((v / 20.0f) * h);
					NanoVg.NvgLineTo(vg, vx, vy);
				}
			}
			NanoVg.NvgLineTo(vg, x + w, y + h);
			NanoVg.NvgFillColor(vg, NanoVg.NvgRgba(255, 192, 0, 128));
			NanoVg.NvgFill(vg);

			NanoVg.NvgFontFace(vg, "sans");

			if (_name[0] != '\0')
			{
				NanoVg.NvgFontSize(vg, 14.0f);
				NanoVg.NvgTextAlign(vg, (int)(NvgAlign.Left | NvgAlign.Top));
				NanoVg.NvgFillColor(vg, NanoVg.NvgRgba(240, 240, 240, 192));
				NanoVg.NvgText(vg, x + 3, y + 1, _name);
			}

			if (_style == (int)GraphRenderStyle.Fps)
			{
				NanoVg.NvgFontSize(vg, 18.0f);
				NanoVg.NvgTextAlign(vg, (int)(NvgAlign.Right | NvgAlign.Top));
				NanoVg.NvgFillColor(vg, NanoVg.NvgRgba(240, 240, 240, 255));
				str = $"{1.0f / avg:0.00} FPS";
				NanoVg.NvgText(vg, x + w - 3, y + 1, str);

				NanoVg.NvgFontSize(vg, 15.0f);
				NanoVg.NvgTextAlign(vg, (int)(NvgAlign.Right | NvgAlign.Bottom));
				NanoVg.NvgFillColor(vg, NanoVg.NvgRgba(240, 240, 240, 160));
				str = $"{avg * 1000.0f:0.00} ms";
				NanoVg.NvgText(vg, x + w - 3, y + h - 1, str);
			}
			else if (_style == (int)GraphRenderStyle.Percent)
			{
				NanoVg.NvgFontSize(vg, 18.0f);
				NanoVg.NvgTextAlign(vg, (int)(NvgAlign.Right | NvgAlign.Top));
				NanoVg.NvgFillColor(vg, NanoVg.NvgRgba(240, 240, 240, 255));
				str = $"{avg * 1.0f:0.0} %";
				NanoVg.NvgText(vg, x + w - 3, y + 1, str);
			}
			else
			{
				NanoVg.NvgFontSize(vg, 18.0f);
				NanoVg.NvgTextAlign(vg, (int)(NvgAlign.Right | NvgAlign.Top));
				NanoVg.NvgFillColor(vg, NanoVg.NvgRgba(240, 240, 240, 255));
				str = $"{avg * 1000.0f:0.00} ms";
				NanoVg.NvgText(vg, x + w - 3, y + 1, str);
			}
		}
	}
}

