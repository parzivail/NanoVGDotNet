//
// nanosvg-csharp Copyright (c) 2013 by Thinksquirrel Software, LLC
//
// nanosvg Copyright (c) 2009 Mikko Mononen memon@inside.org
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
// The SVG parser is based on Anti-Graim Geometry SVG example
// Copyright (C) 2002-2004 Maxim Shemanarev (McSeem)
//
/* Example Usage:
        // Load
        string svgData = ...;
        SvgPath plist = SvgParser.SvgParse(svgData);
        
        // Use...
        for (SvgPath it = plist; it != null; it = it.next)
            ...
*/
namespace NanoVgTest
{
    public class SvgParser
    {
        public class SvgPath
        {
            public string id;
            public float[] pts;
            public int npts;
            public uint fillColor;
            public uint strokeColor;
            public float strokeWidth;
            public bool hasFill;
            public bool hasStroke;
            public bool closed;
            public SvgPath next;
        };

        #region Added for C# port
        #region Fixed arrays
        struct FixedFloatArray2
        {
            float f0, f1;

            public float this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return f0;
                        case 1:
                            return f1;
                        default:
                            throw new System.IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            f0 = value;
                            break;
                        case 1:
                            f1 = value;
                            break;
                        default:
                            throw new System.IndexOutOfRangeException();
                    }
                }
            }
        }
        struct FixedFloatArray10
        {
            float f0, f1, f2, f3, f4, f5, f6, f7, f8, f9;

            public float this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return f0;
                        case 1:
                            return f1;
                        case 2:
                            return f2;
                        case 3:
                            return f3;
                        case 4:
                            return f4;
                        case 5:
                            return f5;
                        case 6:
                            return f6;
                        case 7:
                            return f7;
                        case 8:
                            return f8;
                        case 9:
                            return f9;
                        default:
                            throw new System.IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            f0 = value;
                            break;
                        case 1:
                            f1 = value;
                            break;
                        case 2:
                            f2 = value;
                            break;
                        case 3:
                            f3 = value;
                            break;
                        case 4:
                            f4 = value;
                            break;
                        case 5:
                            f5 = value;
                            break;
                        case 6:
                            f6 = value;
                            break;
                        case 7:
                            f7 = value;
                            break;
                        case 8:
                            f8 = value;
                            break;
                        case 9:
                            f9 = value;
                            break;
                        default:
                            throw new System.IndexOutOfRangeException();
                    }
                }
            }
        }
        #endregion
        #region Transform struct
        struct Xf
        {
            float m0, m1, m2, m3, m4, m5;

            public float this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return m0;
                        case 1:
                            return m1;
                        case 2:
                            return m2;
                        case 3:
                            return m3;
                        case 4:
                            return m4;
                        case 5:
                            return m5;
                        default:
                            throw new System.IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            m0 = value;
                            break;
                        case 1:
                            m1 = value;
                            break;
                        case 2:
                            m2 = value;
                            break;
                        case 3:
                            m3 = value;
                            break;
                        case 4:
                            m4 = value;
                            break;
                        case 5:
                            m5 = value;
                            break;
                        default:
                            throw new System.IndexOutOfRangeException();
                    }
                }
            }
        }
        #endregion
        #region A C-style mutable string implementation
        struct CString
        {
            int _address;
            CStringData _str;

            public CString(string str)
            {
                _address = 0;
                this._str = new CStringData(str);
                this.HasValue = true;
            }

            public CString(int capacity)
            {
                _address = 0;
                this._str = new CStringData(string.Empty);
                this._str._sb.Capacity = capacity;
                this.HasValue = true;
            }

            CString(CStringData str) : this(0, str) { }

            CString(int i, CStringData str)
            {
                this._address = i;
                this._str = str;
                this.HasValue = true;
            }

            // Use this for null pointer checks, since pointers to 0 are legal
            public readonly bool HasValue;

            public StringChar Ref
            {
                get
                {
                    if (_str == null)
                        return '\0';
                    if (_address < 0 || _address >= _str.Length)
                        return '\0';
                    else return _str[_address];
                }
                set
                {
                    _str[_address] = value;
                }
            }

            public char this[int index]
            {
                get
                {
                    int i = _address + index;
                    if (i < 0 || i >= _str.Length)
                        return '\0';

                    return _str[i];
                }
                set
                {
                    int i = _address + index;

                    if (i >= _str.Length)
                        EnsureMinCapacity(i + 1);

                    _str[i] = value;
                }
            }

            internal void EnsureMinCapacity(int count)
            {
                _str.EnsureMinCapacity(count);
            }

            public static CString operator ++(CString p)
            {
                p._address += 1;
                return p;
            }

            public static CString operator --(CString p)
            {
                p._address -= 1;
                return p;
            }

            public static implicit operator int(CString p)
            {
                return p._address;
            }
            public static CString operator +(CString p, int i)
            {
                p._address += i;
                return p;
            }
            public static CString operator -(CString p, int i)
            {
                p._address -= i;
                return p;
            }
            public static CString operator +(int i, CString p)
            {
                p._address = i + p._address;
                return p;
            }
            public static CString operator -(int i, CString p)
            {
                p._address = i - p._address;
                return p;
            }
            public static implicit operator bool(CString p)
            {
                return p.HasValue;
            }

            public static implicit operator CString(string s)
            {
                return new CString(s);
            }

            public static implicit operator string(CString s)
            {
                return s.ToString();
            }

            public override string ToString()
            {
                if (_str == null) return null;

                return _str.ToString(_address);
            }

            public static int Compare(CString s1, CString s2)
            {
                byte uc1, uc2;

                /* Move s1 and s2 to the first differing characters 
                in each string, or the ends of the strings if they
                are identical.  */
                while (s1.Ref && s1.Ref == s2.Ref)
                {
                    s1++;
                    s2++;
                }

                /* Compare the characters as unsigned char and
                return the difference.  */
                uc1 = (byte)s1.Ref;
                uc2 = (byte)s2.Ref;
                return (uc1 < uc2) ? -1 : ((uc1 > uc2) ? 1 : 0);
            }

            public static int Compare(CString s1, CString s2, int n)
            {
                byte uc1, uc2;
                int i = 0;

                /* Move s1 and s2 to the first differing characters 
                in each string, or the ends of the strings if they
                are identical.  */
                while (i < n && s1.Ref && s1.Ref == s2.Ref)
                {
                    s1++;
                    s2++;
                    i++;
                }

                /* Compare the characters as unsigned char and
                return the difference.  */
                uc1 = (byte)s1.Ref;
                uc2 = (byte)s2.Ref;
                return (uc1 < uc2) ? -1 : ((uc1 > uc2) ? 1 : 0);
            }

            // Character reference
            public struct StringChar
            {
                public char Value;

                public static implicit operator char(StringChar r)
                {
                    return r.Value;
                }

                public static implicit operator StringChar(char c)
                {
                    StringChar r = new StringChar();
                    r.Value = c;
                    return r;
                }

                public static explicit operator byte(StringChar r)
                {
                    return (byte)r.Value;
                }

                public static bool operator ==(StringChar r1, StringChar r2)
                {
                    return r1.Value == r2.Value;
                }
                public static bool operator !=(StringChar r1, StringChar r2)
                {
                    return r1.Value != r2.Value;
                }
                public static bool operator ==(StringChar r, char c)
                {
                    return r.Value == c;
                }
                public static bool operator !=(StringChar r, char c)
                {
                    return r.Value != c;
                }
                public static bool operator ==(char c, StringChar r)
                {
                    return c == r.Value;
                }
                public static bool operator !=(char c, StringChar r)
                {
                    return c != r.Value;
                }

                public override bool Equals(object o)
                {
                    if (!(o is StringChar))
                        return false;

                    StringChar r = (StringChar)o;

                    return this.Value.Equals(r.Value);
                }
                public override int GetHashCode()
                {
                    return this.Value.GetHashCode();
                }

                public static implicit operator bool(StringChar r)
                {
                    return r != '\0';
                }
            }

            // Backing string data for one or more pointers
            class CStringData
            {
                internal System.Text.StringBuilder _sb;

                public CStringData() : this(string.Empty) { }

                public CStringData(string input)
                {
                    _sb = new System.Text.StringBuilder(input);
                }

                public static implicit operator CStringData(string s)
                {
                    return new CStringData(s);
                }

                public string ToString(int p)
                {
                    int start = p;
                    while (this[p] != '\0')
                    {
                        p++;
                    }

                    return _sb.ToString(start, p - start);
                }

                public int Length
                {
                    get
                    {
                        return _sb.Length;
                    }
                }
                public StringChar this[int index]
                {
                    get
                    {
                        if (index < 0 || index >= _sb.Length)
                            return '\0';
                        else
                            return _sb[index];
                    }
                    set
                    {
                        if (index >= _sb.Length)
                            EnsureMinCapacity(index + 1);

                        _sb[index] = value;
                    }
                }

                public void EnsureMinCapacity(int count)
                {
                    while (_sb.Capacity < count)
                        _sb.Capacity *= 2;

                    if (_sb.Length < count)
                        _sb.Append('\0', count - _sb.Length);
                }
            }
        }
        #endregion
        #region Color names (static constructor)
        static System.Collections.Generic.Dictionary<string, string> _colorNames = new System.Collections.Generic.Dictionary<string, string>();

        static SvgParser()
        {
            _colorNames.Add("aliceblue", "F0F8FF");
            _colorNames.Add("antiquewhite", "FAEBD7");
            _colorNames.Add("aqua", "00FFFF");
            _colorNames.Add("aquamarine", "7FFFD4");
            _colorNames.Add("azure", "F0FFFF");
            _colorNames.Add("beige", "F5F5DC");
            _colorNames.Add("bisque", "FFE4C4");
            _colorNames.Add("black", "000000");
            _colorNames.Add("blanchedalmond", "FFEBCD");
            _colorNames.Add("blue", "0000FF");
            _colorNames.Add("blueviolet", "8A2BE2");
            _colorNames.Add("brown", "A52A2A");
            _colorNames.Add("burlywood", "DEB887");
            _colorNames.Add("cadetblue", "5F9EA0");
            _colorNames.Add("chartreuse", "7FFF00");
            _colorNames.Add("chocolate", "D2691E");
            _colorNames.Add("coral", "FF7F50");
            _colorNames.Add("cornflowerblue", "6495ED");
            _colorNames.Add("cornsilk", "FFF8DC");
            _colorNames.Add("crimson", "DC143C");
            _colorNames.Add("cyan", "00FFFF");
            _colorNames.Add("darkblue", "00008B");
            _colorNames.Add("darkcyan", "008B8B");
            _colorNames.Add("darkgoldenrod", "B8860B");
            _colorNames.Add("darkgray", "A9A9A9");
            _colorNames.Add("darkgreen", "006400");
            _colorNames.Add("darkkhaki", "BDB76B");
            _colorNames.Add("darkmagenta", "8B008B");
            _colorNames.Add("darkolivegreen", "556B2F");
            _colorNames.Add("darkorange", "FF8C00");
            _colorNames.Add("darkorchid", "9932CC");
            _colorNames.Add("darkred", "8B0000");
            _colorNames.Add("darksalmon", "E9967A");
            _colorNames.Add("darkseagreen", "8FBC8F");
            _colorNames.Add("darkslateblue", "483D8B");
            _colorNames.Add("darkslategray", "2F4F4F");
            _colorNames.Add("darkturquoise", "00CED1");
            _colorNames.Add("darkviolet", "9400D3");
            _colorNames.Add("deeppink", "FF1493");
            _colorNames.Add("deepskyblue", "00BFFF");
            _colorNames.Add("dimgray", "696969");
            _colorNames.Add("dimgrey", "696969");
            _colorNames.Add("dodgerblue", "1E90FF");
            _colorNames.Add("firebrick", "B22222");
            _colorNames.Add("floralwhite", "FFFAF0");
            _colorNames.Add("forestgreen", "228B22");
            _colorNames.Add("fuchsia", "FF00FF");
            _colorNames.Add("gainsboro", "DCDCDC");
            _colorNames.Add("ghostwhite", "F8F8FF");
            _colorNames.Add("gold", "FFD700");
            _colorNames.Add("goldenrod", "DAA520");
            _colorNames.Add("gray", "808080");
            _colorNames.Add("green", "008000");
            _colorNames.Add("greenyellow", "ADFF2F");
            _colorNames.Add("honeydew", "F0FFF0");
            _colorNames.Add("hotpink", "FF69B4");
            _colorNames.Add("indianred", "CD5C5C");
            _colorNames.Add("indigo", "4B0082");
            _colorNames.Add("ivory", "FFFFF0");
            _colorNames.Add("khaki", "F0E68C");
            _colorNames.Add("lavender", "E6E6FA");
            _colorNames.Add("lavenderblush", "FFF0F5");
            _colorNames.Add("lawnGreen", "7CFC00");
            _colorNames.Add("lemonchiffon", "FFFACD");
            _colorNames.Add("lightblue", "ADD8E6");
            _colorNames.Add("lightcoral", "F08080");
            _colorNames.Add("lightcyan", "E0FFFF");
            _colorNames.Add("lightgoldenrodyellow", "FAFAD2");
            _colorNames.Add("lightgray", "D3D3D3");
            _colorNames.Add("lightgreen", "90EE90");
            _colorNames.Add("lightpink", "FFB6C1");
            _colorNames.Add("lightsalmon", "FFA07A");
            _colorNames.Add("lightseagreen", "20B2AA");
            _colorNames.Add("lightskyblue", "87CEFA");
            _colorNames.Add("lightslategray", "778899");
            _colorNames.Add("lightsteelblue", "B0C4DE");
            _colorNames.Add("lightyellow", "FFFFE0");
            _colorNames.Add("lime", "00FF00");
            _colorNames.Add("limegreen", "32CD32");
            _colorNames.Add("linen", "FAF0E6");
            _colorNames.Add("magenta", "FF00FF");
            _colorNames.Add("maroon", "800000");
            _colorNames.Add("mediumaquamarine", "66CDAA");
            _colorNames.Add("mediumblue", "0000CD");
            _colorNames.Add("mediumorchid", "BA55D3");
            _colorNames.Add("mediumpurple", "9370DB");
            _colorNames.Add("mediumseagreen", "3CB371");
            _colorNames.Add("mediumslateblue", "7B68EE");
            _colorNames.Add("mediumspringgreen", "00FA9A");
            _colorNames.Add("mediumturquoise", "48D1CC");
            _colorNames.Add("mediumvioletred", "C71585");
            _colorNames.Add("midnightblue", "191970");
            _colorNames.Add("mintcream", "F5FFFA");
            _colorNames.Add("mistyrose", "FFE4E1");
            _colorNames.Add("moccasin", "FFE4B5");
            _colorNames.Add("navajowhite", "FFDEAD");
            _colorNames.Add("navy", "000080");
            _colorNames.Add("oldlace", "FDF5E6");
            _colorNames.Add("olive", "808000");
            _colorNames.Add("olivedrab", "6B8E23");
            _colorNames.Add("orange", "FFA500");
            _colorNames.Add("orangered", "FF4500");
            _colorNames.Add("orchid", "DA70D6");
            _colorNames.Add("palegoldenrod", "EEE8AA");
            _colorNames.Add("palegreen", "98FB98");
            _colorNames.Add("paleturquoise", "AFEEEE");
            _colorNames.Add("palevioletred", "DB7093");
            _colorNames.Add("papayawhip", "FFEFD5");
            _colorNames.Add("peachpuff", "FFDAB9");
            _colorNames.Add("peru", "CD853F");
            _colorNames.Add("pink", "FFC0CB");
            _colorNames.Add("plum", "DDA0DD");
            _colorNames.Add("powderblue", "B0E0E6");
            _colorNames.Add("purple", "800080");
            _colorNames.Add("red", "FF0000");
            _colorNames.Add("rosybrown", "BC8F8F");
            _colorNames.Add("royalblue", "4169E1");
            _colorNames.Add("saddlebrown", "8B4513");
            _colorNames.Add("salmon", "FA8072");
            _colorNames.Add("sandybrown", "F4A460");
            _colorNames.Add("seagreen", "2E8B57");
            _colorNames.Add("seashell", "FFF5EE");
            _colorNames.Add("sienna", "A0522D");
            _colorNames.Add("silver", "C0C0C0");
            _colorNames.Add("skyblue", "87CEEB");
            _colorNames.Add("slateblue", "6A5ACD");
            _colorNames.Add("slategray", "708090");
            _colorNames.Add("snow", "FFFAFA");
            _colorNames.Add("springgreen", "00FF7F");
            _colorNames.Add("steelblue", "4682B4");
            _colorNames.Add("tan", "D2B48C");
            _colorNames.Add("teal", "008080");
            _colorNames.Add("thistle", "D8BFD8");
            _colorNames.Add("tomato", "FF6347");
            _colorNames.Add("turquoise", "40E0D0");
            _colorNames.Add("violet", "EE82EE");
            _colorNames.Add("wheat", "F5DEB3");
            _colorNames.Add("white", "FFFFFF");
            _colorNames.Add("whitesmoke", "F5F5F5");
            _colorNames.Add("yellow", "FFFF00");
            _colorNames.Add("yellowgreen", "9ACD32");
        }
        #endregion
        #region Standard library stuff
        // This is a fake sscanf, just to parse colors
        static void sscanf(CString s, ref uint i)
        {
            string str = s.ToString();

            // 3-digit hex code
            if (str.Length == 3)
                str = string.Concat(str[0], str[0], str[1], str[1], str[2], str[2]);

            // Try to parse
            if (!uint.TryParse(str, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out i))
            {
                // Try a color name
                if (_colorNames.TryGetValue(str.ToLower(System.Globalization.CultureInfo.InvariantCulture), out str))
                {
                    uint.TryParse(str, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out i);
                }
            }
        }

        static double atof(CString s)
        {
            double r;
            double.TryParse(s.ToString(), out r);
            return r;
        }
        static int strcmp(CString a, CString b)
        {
            return CString.Compare(a, b);
        }
        static int strncmp(CString a, CString b, int n)
        {
            return CString.Compare(a, b, n);
        }
        static bool isspace(char c)
        {
            return char.IsWhiteSpace(c);
        }
        static char tolower(char c)
        {
            return char.ToLower(c);
        }
        static float fabsf(float f)
        {
            return System.Math.Abs(f);
        }
        static float acosf(float f)
        {
            return (float)System.Math.Acos(f);
        }
        static float ceilf(float f)
        {
            return (float)System.Math.Ceiling(f);
        }
        static float cosf(float f)
        {
            return (float)System.Math.Cos(f);
        }
        static float sinf(float f)
        {
            return (float)System.Math.Sin(f);
        }
        // Length only applies for arrays and strings and is element length, not bytes.
        static void memcpy<T>(ref T[] destination, ref T[] source, int length)
        {
            if (source == null)
                destination = null;
            else if (destination == null)
                destination = new T[length];
            else
                System.Array.Resize<T>(ref destination, length);

            System.Array.Copy(source, destination, System.Math.Min(source.Length, length));
        }
        static void memcpy(ref CString destination, ref CString source, int length)
        {
            destination.EnsureMinCapacity(length);

            for (int i = 0; i < length; ++i)
            {
                destination[i] = source[i];
            }
        }
        static void memcpy(ref Xf destination, ref Xf source)
        {
            for (int i = 0; i < 6; ++i)
                destination[i] = source[i];
        }
        static void memcpy(ref SvgAttrib destination, ref SvgAttrib source)
        {
            if (source == null)
                destination = null;
            else if (destination == null)
                destination = new SvgAttrib();

            destination.xform = source.xform;
            destination.fillColor = source.fillColor;
            destination.strokeColor = source.strokeColor;
            destination.fillOpacity = source.fillOpacity;
            destination.strokeWidth = source.strokeWidth;
            destination.hasFill = source.hasFill;
            destination.hasStroke = source.hasStroke;
            destination.visible = source.visible;
        }
        #endregion
        #endregion

        const float M_PI = 3.14159265358979323846264338327f;

        // Simple XML parser

        const int TAG = 1;
        const int CONTENT = 2;
        const int MAX_ATTRIBS = 256;

        static void ParseContent(CString s,
                                 System.Action<object, CString> contentCb,
                                 object ud)
        {
            // Trim start white spaces
            while (s.Ref && isspace(s.Ref)) s++;
            if (!s.Ref) return;

            if (contentCb != null)
                contentCb(ud, s);
        }


        static void ParseElement(CString s,
                                 System.Action<object, CString, CString[]> startelCb,
                                 System.Action<object, CString> endelCb,
                                 object ud)
        {
            CString[] attr = new CString[MAX_ATTRIBS];
            int nattr = 0;
            CString name;
            bool start = false;
            bool end = false;

            // Skip white space after the '<'
            while (s.Ref && isspace(s.Ref)) s++;

            // Check if the tag is end tag
            if (s.Ref == '/')
            {
                s++;
                end = true;
            }
            else
            {
                start = true;
            }

            // Skip comments, data and preprocessor stuff.
            if (!s.Ref || s.Ref == '?' || s.Ref == '!')
                return;

            // Get tag name
            name = s;
            while (s.Ref && !isspace(s.Ref)) s++;
            if (s.Ref) { s.Ref = '\0'; s++; }

            // Get attribs
            while (!end && s.Ref && nattr < MAX_ATTRIBS - 1)
            {
                // Skip white space before the attrib name
                while (s.Ref && isspace(s.Ref)) s++;
                if (!s.Ref) break;
                if (s.Ref == '/')
                {
                    end = true;
                    break;
                }
                attr[nattr++] = s;

                // Find end of the attrib name.
                while (s.Ref && !isspace(s.Ref) && s.Ref != '=') s++;
                if (s.Ref) { s.Ref = '\0'; s++; }

                // Skip until the beginning of the value.
                while (s.Ref && s.Ref != '\"') s++;
                if (!s.Ref) break;
                s++;

                // Store value and find the end of it.
                attr[nattr++] = s;
                while (s.Ref && s.Ref != '\"') s++;
                if (s.Ref) { s.Ref = '\0'; s++; }
            }

            // List terminator
            attr[nattr++] = new CString();
            attr[nattr++] = new CString();

            // Call callbacks.
            if (start && startelCb != null)
                startelCb(ud, name, attr);
            if (end && endelCb != null)
                endelCb(ud, name);
        }

        static bool ParseXml(CString input,
                     System.Action<object, CString, CString[]> startelCb,
                     System.Action<object, CString> endelCb,
                     System.Action<object, CString> contentCb,
                     object ud)
        {
            CString s = input;
            CString mark = s;
            int state = CONTENT;

            while (s.Ref)
            {
                if (s.Ref == '<' && state == CONTENT)
                {
                    // Start of a tag
                    s.Ref = '\0'; s++;
                    ParseContent(mark, contentCb, ud);
                    mark = s;
                    state = TAG;
                }
                else if (s.Ref == '>' && state == TAG)
                {
                    // Start of a content or new tag.
                    s.Ref = '\0'; s++;
                    ParseElement(mark, startelCb, endelCb, ud);
                    mark = s;
                    state = CONTENT;
                }
                else
                    s++;
            }

            return true;
        }

        /* Simple SVG parser. */

        const int SVG_MAX_ATTR = 128;

        class SvgAttrib
        {
            public CString id;
            public Xf xform;
            public uint fillColor;
            public uint strokeColor;
            public float fillOpacity;
            public float strokeOpacity;
            public float strokeWidth;
            public bool hasFill;
            public bool hasStroke;
            public bool visible;
        }

        SvgAttrib[] attr = new SvgAttrib[SVG_MAX_ATTR];
        int attrHead;
        System.Collections.Generic.List<float> buf = new System.Collections.Generic.List<float>();
        int nbuf;
        SvgPath plist;
        bool pathFlag;
        bool defsFlag;
        float tol;
        int bez;

        static void XformSetIdentity(ref Xf t)
        {
            t[0] = 1.0f; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = 1.0f;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        static void XformSetTranslation(ref Xf t, float tx, float ty)
        {
            t[0] = 1.0f; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = 1.0f;
            t[4] = tx; t[5] = ty;
        }

        static void XformSetScale(ref Xf t, float sx, float sy)
        {
            t[0] = sx; t[1] = 0.0f;
            t[2] = 0.0f; t[3] = sy;
            t[4] = 0.0f; t[5] = 0.0f;
        }

        static void XformMultiply(ref Xf t, ref Xf s)
        {
            float t0 = t[0] * s[0] + t[1] * s[2];
            float t2 = t[2] * s[0] + t[3] * s[2];
            float t4 = t[4] * s[0] + t[5] * s[2] + s[4];
            t[1] = t[0] * s[1] + t[1] * s[3];
            t[3] = t[2] * s[1] + t[3] * s[3];
            t[5] = t[4] * s[1] + t[5] * s[3] + s[5];
            t[0] = t0;
            t[2] = t2;
            t[4] = t4;
        }

        static void XformPremultiply(ref Xf t, ref Xf s)
        {
            Xf s2 = new Xf();
            memcpy(ref s2, ref s);
            XformMultiply(ref s2, ref t);
            memcpy(ref t, ref s2);
        }

        public SvgParser()
        {
            // Init style
            for (int i = 0; i < this.attr.Length; ++i)
            {
                this.attr[i] = new SvgAttrib();

                XformSetIdentity(ref this.attr[i].xform);
                this.attr[i].id = string.Empty;
                this.attr[i].fillColor = 0;
                this.attr[i].strokeColor = 0;
                this.attr[i].fillOpacity = 1;
                this.attr[i].strokeOpacity = 1;
                this.attr[i].strokeWidth = 1;
                this.attr[i].hasFill = true;
                this.attr[i].hasStroke = false;
                this.attr[i].visible = true;
            }
        }

        static void SvgResetPath(SvgParser p)
        {
            p.nbuf = 0;
        }

        static void SvgPathPoint(SvgParser p, float x, float y)
        {
            if (p.nbuf == 0) p.buf.Clear();
            p.buf.Add(x);
            p.buf.Add(y);
            p.nbuf++;
        }

        static SvgAttrib SvgGetAttr(SvgParser p)
        {
            return p.attr[p.attrHead];
        }

        static void SvgPushAttr(SvgParser p)
        {
            if (p.attrHead < SVG_MAX_ATTR - 1)
            {
                p.attrHead++;
                memcpy(ref p.attr[p.attrHead], ref p.attr[p.attrHead - 1]);
            }
        }

        static void SvgPopAttr(SvgParser p)
        {
            if (p.attrHead > 0)
                p.attrHead--;
        }

        static void SvgCreatePath(SvgParser p, bool closed)
        {
            Xf t;
            SvgAttrib attr;
            SvgPath path;
            int i;

            if (p == null)
                return;

            if (p.nbuf == 0)
            {
                return;
            }

            attr = SvgGetAttr(p);

            path = new SvgPath();
            path.pts = new float[p.nbuf * 2];

            path.closed = closed;
            path.npts = p.nbuf;

            path.next = p.plist;
            p.plist = path;

            // Transform path.
            t = attr.xform;
            for (i = 0; i < p.nbuf; ++i)
            {
                path.pts[i * 2 + 0] = p.buf[i * 2 + 0] * t[0] + p.buf[i * 2 + 1] * t[2] + t[4];
                path.pts[i * 2 + 1] = p.buf[i * 2 + 0] * t[1] + p.buf[i * 2 + 1] * t[3] + t[5];
            }

            path.id = attr.id.ToString();
            path.hasFill = attr.hasFill;
            path.hasStroke = attr.hasStroke;
            path.strokeWidth = attr.strokeWidth * t[0];

            path.fillColor = attr.fillColor;
            if (path.hasFill)
                path.fillColor |= (uint)(attr.fillOpacity * 255) << 24;

            path.strokeColor = attr.strokeColor;
            if (path.hasStroke)
                path.strokeColor |= (uint)(attr.strokeOpacity * 255) << 24;
        }

        static bool isnum(char c)
        {
            return "0123456789+-.eE".IndexOf(c) != -1;
        }

        // Port note - was already commented. Ported it anyways
        /* static CString ParsePathFloats(CString s, float[] arg, int n)
        {
            CString num = new CString(64);
            CString start;
            int nnum;
            int i = 0;
            while (s.Ref && i < n)
            {
                // Skip white spaces and commas
                while (s.Ref && (isspace(s.Ref) || s.Ref == ',')) s++;
                if (!s.Ref) break;
                start = s;
                nnum = 0;
                while (s.Ref && isnum(s.Ref))
                {
                        if (nnum < 63) num[nnum++] = s.Ref;
                        s++;
                }
                num[nnum] = '\0';
                arg[i++] = (float)atof(num);
            }
            return s;
        } */

        static CString GetNextPathItem(CString s, CString it)
        {
            int i = 0;
            it[0] = '\0';
            // Skip white spaces and commas
            while (s.Ref && (isspace(s.Ref) || s.Ref == ',')) s++;
            if (!s.Ref) return s;
            if (s.Ref == '-' || s.Ref == '+' || isnum(s.Ref))
            {
                while (s.Ref == '-' || s.Ref == '+')
                {
                    if (i < 63) it[i++] = s.Ref;
                    s++;
                }
                while (s.Ref && s.Ref != '-' && s.Ref != '+' && isnum(s.Ref))
                {
                    if (i < 63) it[i++] = s.Ref;
                    s++;
                }
                it[i] = '\0';
            }
            else
            {
                it[0] = s.Ref; s++;
                it[1] = '\0';
                return s;
            }
            return s;
        }

        static CString ParseName(CString str)
        {
            return str;
        }

        static uint ParseColor(CString str)
        {
            uint c = 0;
            while (str.Ref == ' ') ++str;
            if (str.Ref == '#')
            {
                sscanf(str + 1, ref c);
            }
            return c;
        }

        static float ParseFloat(CString str)
        {
            while (str.Ref == ' ') ++str;
            return (float)atof(str);
        }

        static int ParseTransformArgs(CString str, Xf args, int maxNa, ref int na)
        {
            CString end;
            CString ptr;

            na = 0;
            ptr = str;
            while (ptr.Ref && ptr.Ref != '(') ++ptr;
            if (!ptr.HasValue) // Port note: was ptr == 0
                return 1;
            end = ptr;
            while (end.Ref && end.Ref != ')') ++end;
            if (!end.HasValue) // Port note: was end == 0
                return 1;

            while (ptr < end)
            {
                if (isnum(ptr.Ref))
                {
                    if (na >= maxNa) return 0;
                    args[na++] = (float)atof(ptr);
                    while (ptr < end && isnum(ptr.Ref)) ++ptr;
                }
                else
                {
                    ++ptr;
                }
            }
            return ((int)end - (int)str); // Port note: was return (int)(end - str);
        }

        static int SvgParseMatrix(SvgParser p, CString str)
        {
            Xf t = new Xf();
            int na = 0;
            int len = ParseTransformArgs(str, t, 6, ref na);
            if (na != 6) return len;
            XformPremultiply(ref SvgGetAttr(p).xform, ref t);
            return len;
        }

        static int SvgParseTranslate(SvgParser p, CString str)
        {
            Xf args = new Xf();
            Xf t = new Xf();
            int na = 0;
            int len = ParseTransformArgs(str, args, 2, ref na);
            if (na == 1) args[1] = 0.0f;
            XformSetTranslation(ref t, args[0], args[1]);
            XformPremultiply(ref SvgGetAttr(p).xform, ref t);
            return len;
        }

        static int SvgParseScale(SvgParser p, CString str)
        {
            Xf args = new Xf();
            int na = 0;
            Xf t = new Xf();
            int len = ParseTransformArgs(str, args, 2, ref na);
            if (na == 1) args[1] = args[0];
            XformSetScale(ref t, args[0], args[1]);
            XformPremultiply(ref SvgGetAttr(p).xform, ref t);
            return len;
        }

        static void SvgParseTransform(SvgParser p, CString str)
        {
            while (str.Ref)
            {
                if (strncmp(str, "matrix", 6) == 0)
                    str += SvgParseMatrix(p, str);
                else if (strncmp(str, "translate", 9) == 0)
                    str += SvgParseTranslate(p, str);
                else if (strncmp(str, "scale", 5) == 0)
                    str += SvgParseScale(p, str);
                else
                    ++str;
            }
        }

        static bool SvgParseAttr(SvgParser p, CString name, CString value)
        {
            SvgAttrib attr = SvgGetAttr(p);
            if (attr == null) return false;

            if (strcmp(name, "style") == 0)
            {
                SvgParseStyle(p, value);
            }
            else if (strcmp(name, "display") == 0)
            {
                if (strcmp(value, "none") == 0)
                    attr.visible = false;
                else
                    attr.visible = true;
            }
            else if (strcmp(name, "opacity") == 0)
            {
                attr.fillOpacity = ParseFloat(value);
                attr.strokeOpacity = attr.fillOpacity;
            }
            else if (strcmp(name, "id") == 0)
            {
                attr.id = ParseName(value);
            }
            else if (strcmp(name, "fill") == 0)
            {
                if (strcmp(value, "none") == 0)
                {
                    attr.hasFill = false;
                }
                else
                {
                    attr.hasFill = true;
                    attr.fillColor = ParseColor(value);
                }
            }
            else if (strcmp(name, "fill-opacity") == 0)
            {
                attr.fillOpacity = ParseFloat(value);
            }
            else if (strcmp(name, "stroke") == 0)
            {
                if (strcmp(value, "none") == 0)
                {
                    attr.hasStroke = false;
                }
                else
                {
                    attr.hasStroke = true;
                    attr.strokeColor = ParseColor(value);
                }
            }
            else if (strcmp(name, "stroke-width") == 0)
            {
                attr.strokeWidth = ParseFloat(value);
            }
            else if (strcmp(name, "stroke-opacity") == 0)
            {
                attr.strokeOpacity = ParseFloat(value);
            }
            else if (strcmp(name, "transform") == 0)
            {
                SvgParseTransform(p, value);
            }
            else
            {
                return false;
            }
            return true;
        }

        static bool SvgParseNameValue(SvgParser p, CString start, CString end)
        {
            CString str;
            CString val;
            CString name = new CString(512);
            CString value = new CString(512);
            int n;

            str = start;
            while (str < end && str.Ref != ':') ++str;

            val = str;

            // Right Trim
            while (str > start && (str.Ref == ':' || isspace(str.Ref))) --str;
            ++str;

            n = (int)str - (int)start; // Port note: was n = (int)(str - start);
            if (n > 511) n = 511;
            if (n != 0) memcpy(ref name, ref start, n);
            name[n] = (char)0;

            while (val < end && (val.Ref == ':' || isspace(val.Ref))) ++val;

            n = (int)end - (int)val; // Port note: was n = (int)(end - val);
            if (n > 511) n = 511;
            if (n != 0) memcpy(ref value, ref val, n);
            value[n] = (char)0;

            return SvgParseAttr(p, name, value);
        }

        static void SvgParseStyle(SvgParser p, CString str)
        {
            CString start;
            CString end;

            while (str.Ref)
            {
                // Left Trim
                while (str.Ref && isspace(str.Ref)) ++str;
                start = str;
                while (str.Ref && str.Ref != ';') ++str;
                end = str;

                // Right Trim
                while (end > start && (end.Ref == ';' || isspace(end.Ref))) --end;
                ++end;

                SvgParseNameValue(p, start, end);
                if (str.Ref) ++str;
            }
        }

        static void SvgParseAttribs(SvgParser p, CString[] attr)
        {
            int i;
            for (i = 0; attr[i].HasValue; i += 2)
            {
                if (strcmp(attr[i], "style") == 0)
                    SvgParseStyle(p, attr[i + 1]);
                else
                {
                    SvgParseAttr(p, attr[i], attr[i + 1]);
                }
            }
        }

        static int GetArgsPerElement(char cmd)
        {
            switch (tolower(cmd))
            {
                case 'v':
                case 'h':
                    return 1;
                case 'm':
                case 'l':
                case 't':
                    return 2;
                case 'q':
                case 's':
                    return 4;
                case 'c':
                    return 6;
                case 'a':
                    return 7;
            }
            return 0;
        }

        static float DistPtSeg(float x, float y, float px, float py, float qx, float qy)
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

        static void CubicBezRec(SvgParser p,
                                float x1, float y1, float x2, float y2,
                                float x3, float y3, float x4, float y4,
                                int level)
        {
            float x12, y12, x23, y23, x34, y34, x123, y123, x234, y234, x1234, y1234;
            float d;

            if (level > p.bez) return;

            x12 = (x1 + x2) * 0.5f;
            y12 = (y1 + y2) * 0.5f;
            x23 = (x2 + x3) * 0.5f;
            y23 = (y2 + y3) * 0.5f;
            x34 = (x3 + x4) * 0.5f;
            y34 = (y3 + y4) * 0.5f;
            x123 = (x12 + x23) * 0.5f;
            y123 = (y12 + y23) * 0.5f;
            x234 = (x23 + x34) * 0.5f;
            y234 = (y23 + y34) * 0.5f;
            x1234 = (x123 + x234) * 0.5f;
            y1234 = (y123 + y234) * 0.5f;

            d = DistPtSeg(x1234, y1234, x1, y1, x4, y4);
            if (level > 0 && d < p.tol * p.tol)
            {
                SvgPathPoint(p, x1234, y1234);
                return;
            }

            CubicBezRec(p, x1, y1, x12, y12, x123, y123, x1234, y1234, level + 1);
            CubicBezRec(p, x1234, y1234, x234, y234, x34, y34, x4, y4, level + 1);
        }

        static void CubicBez(SvgParser p,
                             float x1, float y1, float cx1, float cy1,
                             float cx2, float cy2, float x2, float y2)
        {
            CubicBezRec(p, x1, y1, cx1, cy1, cx2, cy2, x2, y2, 0);
            SvgPathPoint(p, x2, y2);
        }

        static void QuadBezRec(SvgParser p,
                               float x1, float y1, float x2, float y2, float x3, float y3,
                               int level)
        {
            float x12, y12, x23, y23, x123, y123, d;

            if (level > p.bez) return;

            x12 = (x1 + x2) * 0.5f;
            y12 = (y1 + y2) * 0.5f;
            x23 = (x2 + x3) * 0.5f;
            y23 = (y2 + y3) * 0.5f;
            x123 = (x12 + x23) * 0.5f;
            y123 = (y12 + y23) * 0.5f;

            d = DistPtSeg(x123, y123, x1, y1, x3, y3);
            if (level > 0 && d < p.tol * p.tol)
            {
                SvgPathPoint(p, x123, y123);
                return;
            }

            QuadBezRec(p, x1, y1, x12, y12, x123, y123, level + 1);
            QuadBezRec(p, x123, y123, x23, y23, x3, y3, level + 1);
        }

        static void QuadBez(SvgParser p,
                            float x1, float y1, float cx, float cy, float x2, float y2)
        {
            QuadBezRec(p, x1, y1, cx, cy, x2, y2, 0);
            SvgPathPoint(p, x2, y2);
        }

        static void PathLineTo(SvgParser p, ref float cpx, ref float cpy, FixedFloatArray10 args, bool rel)
        {
            if (rel)
            {
                cpx += args[0];
                cpy += args[1];
            }
            else
            {
                cpx = args[0];
                cpy = args[1];
            }
            SvgPathPoint(p, cpx, cpy);
        }

        static void PathHLineTo(SvgParser p, ref float cpx, ref float cpy, FixedFloatArray10 args, bool rel)
        {
            if (rel)
                cpx += args[0];
            else
                cpx = args[0];
            SvgPathPoint(p, cpx, cpy);
        }

        static void PathVLineTo(SvgParser p, ref float cpx, ref float cpy, FixedFloatArray10 args, bool rel)
        {
            if (rel)
                cpy += args[0];
            else
                cpy = args[0];
            SvgPathPoint(p, cpx, cpy);
        }

        static void PathCubicBezTo(SvgParser p, ref float cpx, ref float cpy,
                                   ref float cpx2, ref float cpy2, FixedFloatArray10 args, bool rel)
        {
            float x1, y1, x2, y2, cx1, cy1, cx2, cy2;

            x1 = cpx;
            y1 = cpy;
            if (rel)
            {
                cx1 = cpx + args[0];
                cy1 = cpy + args[1];
                cx2 = cpx + args[2];
                cy2 = cpy + args[3];
                x2 = cpx + args[4];
                y2 = cpy + args[5];
            }
            else
            {
                cx1 = args[0];
                cy1 = args[1];
                cx2 = args[2];
                cy2 = args[3];
                x2 = args[4];
                y2 = args[5];
            }

            CubicBez(p, x1, y1, cx1, cy1, cx2, cy2, x2, y2);

            cpx2 = cx2;
            cpy2 = cy2;
            cpx = x2;
            cpy = y2;
        }

        static void PathCubicBezShortTo(SvgParser p, ref float cpx, ref float cpy,
                                        ref float cpx2, ref float cpy2, FixedFloatArray10 args, bool rel)
        {
            float x1, y1, x2, y2, cx1, cy1, cx2, cy2;

            x1 = cpx;
            y1 = cpy;
            if (rel)
            {
                cx2 = cpx + args[0];
                cy2 = cpy + args[1];
                x2 = cpx + args[2];
                y2 = cpy + args[3];
            }
            else
            {
                cx2 = args[0];
                cy2 = args[1];
                x2 = args[2];
                y2 = args[3];
            }

            cx1 = 2 * x1 - cpx2;
            cy1 = 2 * y1 - cpy2;

            CubicBez(p, x1, y1, cx1, cy1, cx2, cy2, x2, y2);

            cpx2 = cx2;
            cpy2 = cy2;
            cpx = x2;
            cpy = y2;
        }

        static void PathQuadBezTo(SvgParser p, ref float cpx, ref float cpy,
                                  ref float cpx2, ref float cpy2, FixedFloatArray10 args, bool rel)
        {
            float x1, y1, x2, y2, cx, cy;

            x1 = cpx;
            y1 = cpy;
            if (rel)
            {
                cx = cpx + args[0];
                cy = cpy + args[1];
                x2 = cpx + args[2];
                y2 = cpy + args[3];
            }
            else
            {
                cx = args[0];
                cy = args[1];
                x2 = args[2];
                y2 = args[3];
            }

            QuadBez(p, x1, y1, cx, cy, x2, y2);

            cpx2 = cx;
            cpy2 = cy;
            cpx = x2;
            cpy = y2;
        }

        static void PathQuadBezShortTo(SvgParser p, ref float cpx, ref float cpy,
                                       ref float cpx2, ref float cpy2, FixedFloatArray10 args, bool rel)
        {
            float x1, y1, x2, y2, cx, cy;

            x1 = cpx;
            y1 = cpy;
            if (rel)
            {
                x2 = cpx + args[0];
                y2 = cpy + args[1];
            }
            else
            {
                x2 = args[0];
                y2 = args[1];
            }

            cx = 2 * x1 - cpx2;
            cy = 2 * y1 - cpy2;

            QuadBez(p, x1, y1, cx, cy, x2, y2);

            cpx2 = cx;
            cpy2 = cy;
            cpx = x2;
            cpy = y2;
        }

        static void SvgParsePath(SvgParser p, CString[] attr)
        {
            CString s;
            char cmd = default(char);
            FixedFloatArray10 args = new FixedFloatArray10();
            int nargs;
            int rargs = 0;
            float cpx = 0, cpy = 0, cpx2 = 0, cpy2 = 0;
            CString[] tmp = new CString[4];
            bool closedFlag;
            int i;
            CString item = new CString(64);

            for (i = 0; attr[i].HasValue; i += 2)
            {
                if (strcmp(attr[i], "d") == 0)
                {
                    s = attr[i + 1];

                    SvgResetPath(p);
                    closedFlag = false;
                    nargs = 0;

                    while (s.Ref)
                    {
                        s = GetNextPathItem(s, item);
                        if (!item.Ref) break;

                        if (isnum(item[0]))
                        {
                            if (nargs < 10)
                                args[nargs++] = (float)atof(item);
                            if (nargs >= rargs)
                            {
                                switch (cmd)
                                {
                                    case 'm':
                                    case 'M':
                                    case 'l':
                                    case 'L':
                                        PathLineTo(p, ref cpx, ref cpy, args, (cmd == 'm' || cmd == 'l') ? true : false);
                                        break;
                                    case 'H':
                                    case 'h':
                                        PathHLineTo(p, ref cpx, ref cpy, args, cmd == 'h' ? true : false);
                                        break;
                                    case 'V':
                                    case 'v':
                                        PathVLineTo(p, ref cpx, ref cpy, args, cmd == 'v' ? true : false);
                                        break;
                                    case 'C':
                                    case 'c':
                                        PathCubicBezTo(p, ref cpx, ref cpy, ref cpx2, ref cpy2, args, cmd == 'c' ? true : false);
                                        break;
                                    case 'S':
                                    case 's':
                                        PathCubicBezShortTo(p, ref cpx, ref cpy, ref cpx2, ref cpy2, args, cmd == 's' ? true : false);
                                        break;
                                    case 'Q':
                                    case 'q':
                                        PathQuadBezTo(p, ref cpx, ref cpy, ref cpx2, ref cpy2, args, cmd == 'q' ? true : false);
                                        break;
                                    case 'T':
                                    case 't':
                                        PathQuadBezShortTo(p, ref cpx, ref cpy, ref cpx2, ref cpy2, args, cmd == 't' ? true : false); // Port note: Was cmd == 's'
                                        break;
                                    default:
                                        if (nargs >= 2)
                                        {
                                            cpx = args[nargs - 2];
                                            cpy = args[nargs - 1];
                                        }
                                        break;
                                }
                                nargs = 0;
                            }
                        }
                        else
                        {
                            cmd = item[0];
                            rargs = GetArgsPerElement(cmd);
                            if (cmd == 'M' || cmd == 'm')
                            {
                                // Commit path.
                                if (p.nbuf != 0)
                                    SvgCreatePath(p, closedFlag);
                                // Start new subpath.
                                SvgResetPath(p);
                                closedFlag = false;
                                nargs = 0;
                                cpx = 0; cpy = 0;
                            }
                            else if (cmd == 'Z' || cmd == 'z')
                            {
                                closedFlag = true;
                                // Commit path.
                                if (p.nbuf != 0)
                                    SvgCreatePath(p, closedFlag);
                                // Start new subpath.
                                SvgResetPath(p);
                                closedFlag = false;
                                nargs = 0;
                            }
                        }
                    }

                    // Commit path.
                    if (p.nbuf != 0)
                        SvgCreatePath(p, closedFlag);

                }
                else
                {
                    tmp[0] = attr[i];
                    tmp[1] = attr[i + 1];
                    tmp[2] = new CString();
                    tmp[3] = new CString();
                    SvgParseAttribs(p, tmp);
                }
            }
        }

        static void SvgParseRect(SvgParser p, CString[] attr)
        {
            float x = 0.0f;
            float y = 0.0f;
            float w = 0.0f;
            float h = 0.0f;
            int i;

            for (i = 0; attr[i].HasValue; i += 2)
            {
                if (!SvgParseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], "x") == 0) x = ParseFloat(attr[i + 1]);
                    if (strcmp(attr[i], "y") == 0) y = ParseFloat(attr[i + 1]);
                    if (strcmp(attr[i], "width") == 0) w = ParseFloat(attr[i + 1]);
                    if (strcmp(attr[i], "height") == 0) h = ParseFloat(attr[i + 1]);
                }
            }

            if (w != 0.0f && h != 0.0f)
            {
                SvgResetPath(p);

                SvgPathPoint(p, x, y);
                SvgPathPoint(p, x + w, y);
                SvgPathPoint(p, x + w, y + h);
                SvgPathPoint(p, x, y + h);

                SvgCreatePath(p, true);
            }
        }

        static void SvgParseCircle(SvgParser p, CString[] attr)
        {
            float cx = 0.0f;
            float cy = 0.0f;
            float r = 0.0f;
            float da;
            int i, n;
            float x, y, u;

            for (i = 0; attr[i].HasValue; i += 2)
            {
                if (!SvgParseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], "cx") == 0) cx = ParseFloat(attr[i + 1]);
                    if (strcmp(attr[i], "cy") == 0) cy = ParseFloat(attr[i + 1]);
                    if (strcmp(attr[i], "r") == 0) r = fabsf(ParseFloat(attr[i + 1]));
                }
            }

            if (r != 0.0f)
            {
                SvgResetPath(p);

                da = acosf(r / (r + p.tol)) * 2;
                n = (int)ceilf(M_PI * 2 / da);

                da = (float)(M_PI * 2) / n;
                for (i = 0; i < n; ++i)
                {
                    u = i * da;
                    x = cx + cosf(u) * r;
                    y = cy + sinf(u) * r;
                    SvgPathPoint(p, x, y);
                }

                SvgCreatePath(p, true);
            }
        }

        static void SvgParseLine(SvgParser p, CString[] attr)
        {
            float x1 = 0.0f;
            float y1 = 0.0f;
            float x2 = 0.0f;
            float y2 = 0.0f;
            int i;

            for (i = 0; attr[i].HasValue; i += 2)
            {
                if (!SvgParseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], "x1") == 0) x1 = ParseFloat(attr[i + 1]);
                    if (strcmp(attr[i], "y1") == 0) y1 = ParseFloat(attr[i + 1]);
                    if (strcmp(attr[i], "x2") == 0) x2 = ParseFloat(attr[i + 1]);
                    if (strcmp(attr[i], "y2") == 0) y2 = ParseFloat(attr[i + 1]);
                }
            }

            SvgResetPath(p);

            SvgPathPoint(p, x1, y1);
            SvgPathPoint(p, x2, y2);

            SvgCreatePath(p, false);
        }

        static void SvgParsePoly(SvgParser p, CString[] attr, bool closeFlag)
        {
            int i;
            CString s;
            FixedFloatArray2 args = new FixedFloatArray2();
            int nargs;
            CString item = new CString(64);

            SvgResetPath(p);

            for (i = 0; attr[i].HasValue; i += 2)
            {
                if (!SvgParseAttr(p, attr[i], attr[i + 1]))
                {
                    if (strcmp(attr[i], "points") == 0)
                    {
                        s = attr[i + 1];
                        nargs = 0;
                        while (s.Ref)
                        {
                            s = GetNextPathItem(s, item);
                            args[nargs++] = (float)atof(item);
                            if (nargs >= 2)
                            {
                                SvgPathPoint(p, args[0], args[1]);
                                nargs = 0;
                            }
                        }
                    }
                }
            }

            SvgCreatePath(p, closeFlag);
        }


        static void SvgStartElement(object ud, CString el, CString[] attr)
        {
            SvgParser p = (SvgParser)ud;

            // Skip everything in defs
            if (p.defsFlag)
                return;

            if (strcmp(el, "g") == 0)
            {
                SvgPushAttr(p);
                SvgParseAttribs(p, attr);
            }
            else if (strcmp(el, "path") == 0)
            {
                if (p.pathFlag)        // Do not allow nested paths.
                    return;
                SvgPushAttr(p);
                SvgParsePath(p, attr);
                p.pathFlag = true;
                SvgPopAttr(p);
            }
            else if (strcmp(el, "rect") == 0)
            {
                SvgPushAttr(p);
                SvgParseRect(p, attr);
                SvgPopAttr(p);
            }
            else if (strcmp(el, "circle") == 0)
            {
                SvgPushAttr(p);
                SvgParseCircle(p, attr);
                SvgPopAttr(p);
            }
            else if (strcmp(el, "line") == 0)
            {
                SvgPushAttr(p);
                SvgParseLine(p, attr);
                SvgPopAttr(p);
            }
            else if (strcmp(el, "polyline") == 0)
            {
                SvgPushAttr(p);
                SvgParsePoly(p, attr, false);
                SvgPopAttr(p);
            }
            else if (strcmp(el, "polygon") == 0)
            {
                SvgPushAttr(p);
                SvgParsePoly(p, attr, true);
                SvgPopAttr(p);
            }
            else if (strcmp(el, "defs") == 0)
            {
                p.defsFlag = true;
            }
        }

        static void SvgEndElement(object ud, CString el)
        {
            SvgParser p = (SvgParser)ud;

            if (strcmp(el, "g") == 0)
            {
                SvgPopAttr(p);
            }
            else if (strcmp(el, "path") == 0)
            {
                p.pathFlag = false;
            }
            else if (strcmp(el, "defs") == 0)
            {
                p.defsFlag = false;
            }
        }

        static void SvgContent(object ud, CString s)
        {
            // empty
        }

        #region Public API
        // Parses SVG file from a string, returns linked list of paths.
        public static SvgPath SvgParse(string input)
        {
            return SvgParse(input, 1.0f, 12);
        }

        // Parses SVG file from a string with given parameters, returns linked list of paths.
        public static SvgPath SvgParse(string input, float tol, int bez)
        {
            SvgParser p;
            SvgPath ret = null;

            p = new SvgParser();
            p.tol = tol;
            p.bez = bez;

            ParseXml(input, SvgStartElement, SvgEndElement, SvgContent, p);

            ret = p.plist;

            return ret;
        }
        #endregion
    }
}