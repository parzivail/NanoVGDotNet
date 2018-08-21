using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace NanoVgTest.Shader
{
    public class DefaultShaderProgram : ShaderProgram
    {
        private readonly string _frag;
        private readonly string _vert;

        public DefaultShaderProgram(string frag, string vert)
        {
            _frag = frag;
            _vert = vert;
        }

        protected override void Init()
        {
            LoadShader(_frag, ShaderType.FragmentShader, PgmId, out FsId);
            LoadShader(_vert, ShaderType.VertexShader, PgmId, out VsId);

            GL.LinkProgram(PgmId);
            Log(GL.GetProgramInfoLog(PgmId));
        }
    }
}
