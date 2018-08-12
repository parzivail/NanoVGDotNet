
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

namespace NanoVGDotNet
{
    public enum NvgCommands
    {
        MoveTo = 0,
        LineTo = 1,
        BezierTo = 2,
        Close = 3,
        Winding = 4
    }

    [Flags]
    public enum NvgPointFlags
    {
        Corner = 1,
        Left = 2,
        Bevel = 4,
        InnerBevel = 8
    }

    [Flags]
    public enum NvgImageFlags
    {
        // Generate mipmaps during creation of the image.
        GenerateMipmaps = 1,
        // Repeat image in X direction.
        RepeatX = 2,
        // Repeat image in Y direction.
        RepeatY = 4,
        // Flips (inverses) image in Y direction when rendered.
        FlipY = 8,
        // Image data has premultiplied alpha.
        Premultiplied = 16
    }

    public enum NvgTexture
    {
        Alpha = 1,
        Rgba = 2
    }

    public enum NvgCompositeOperation
    {
        SourceOver,
        SourceIn,
        SourceOut,
        Atop,
        DestinationOver,
        DestinationIn,
        DestinationOut,
        DestinationAtop,
        Lighter,
        Copy,
        Xor
    }

    [Flags]
    public enum NvgBlendFactor
    {
        Zero = 1,
        One = 2,
        SrcColor = 4,
        OneMinusSrcColor = 8,
        DstColor = 16,
        OneMinusDstColor = 32,
        SrcAlpha = 64,
        OneMinusSrcAlpha = 128,
        DstAlpha = 256,
        OneMinusDstAlpha = 512,
        SrcAlphaSaturate = 1024
    }

    public enum NvgLineCap
    {
        Butt,
        Round,
        Square,
        Bevel,
        Miter
    }

    [Flags]
    public enum NvgAlign
    {
        // Default, align text horizontally to left.
        Left = 1,
        // Align text horizontally to center.
        Center = 2,
        // Align text horizontally to right.
        Right = 4,
        // Align text vertically to top.
        Top = 8,
        // Align text vertically to middle.
        Middle = 16,
        // Align text vertically to bottom.
        Bottom = 32,
        // Default, align text vertically to baseline.
        Baseline = 64
    }

    [Flags]
    public enum NvgCreateFlags
    {
        // Flag indicating if geometry based anti-aliasing is used (may not be needed when using MSAA).
        AntiAlias = 1,
        // Flag indicating if strokes should be drawn using stencil buffer. The rendering will be a little
        // slower, but path overlaps (i.e. self-intersecting or sharp turns) will be drawn just once.
        StencilStrokes = 2,
        // Flag indicating that additional debug checks are done.
        Debug = 4
    }

    // These are additional flags on top of NVGimageFlags.
    [Flags]
    public enum NvgImageFlagsGl
    {
        // Do not delete GL texture handle.
        NoDelete = 65536
    }

    public enum GlNvgUniformLoc
    {
        LocViewSize,
        LocTex,
        LocFrag,
        MaxLocs
    }

    public enum GlNvgCallType
    {
        None,
        Fill,
        ConvexFill,
        Stroke,
        Triangles
    }

    public enum NvgSolidity
    {
        // CCW
        Solid = 1,
        // CW
        Hole = 2
    }

    public enum NvgWinding
    {
        // Winding for solid shapes
        CounterClockwise = 1,
        // Winding for holes
        Clockwise = 2
    }

    public enum GlNvgShaderType
    {
        FillGradient,
        FillImage,
        FillSimple,
        ShaderImage
    }

    public enum GraphRenderStyle
    {
        Fps,
        Milliseconds,
        Percent
    }

    public enum NvgCodepointType
    {
        Space,
        Newline,
        Char
    }
}

