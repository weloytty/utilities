using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ComPortControl
{
    class Program
    {
        static void Main(string[] args)
        {

            foreach (var s in System.IO.Ports.SerialPort.GetPortNames()) {
                Console.WriteLine(s);
            } 
        }
    }
}
