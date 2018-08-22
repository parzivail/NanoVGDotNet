using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace NanoVgTest
{
    class Program
    {
        static void Main(string[] args)
        {
            new MainWindow(new DemoWindow()).Run(20);
        }
    }
}
