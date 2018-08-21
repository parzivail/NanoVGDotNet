using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoVgTest.Shader
{
    public class Uniform
    {
        public Uniform(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public object Value { get; set; }

        public virtual object GetValue()
        {
            return Value;
        }
    }
}
