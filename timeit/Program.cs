using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace timeit {
    class Program {
        static void Main(string[] args) {

            var fileName = args[0];
            if (!File.Exists(fileName)) {
                Console.WriteLine($"Can't find {fileName}");
                return;
            }

            ProcessCreator.CreateProcess(0, args[0]);
        }
    }
}
