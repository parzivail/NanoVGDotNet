namespace NanoVGDotNet
{
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
}