namespace NanoVGDotNet
{
    public class NvGpoint
    {
        public float X, Y;
        public float Dx, Dy;
        public float Len;
        public float Dmx, Dmy;
        public byte Flags;

        public override string ToString()
        {
            return $"[NVGpoint]x={X}, y={Y}, dx={Dx}, dy={Dy}, len={Len}, dmx={Dmx}, dmy={Dmy}";
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
}