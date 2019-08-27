using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.IO;

namespace DeExif
{
    class Program
    {
        static int Main(string[] args)
        {

            return CommandLine.Parser.Default.ParseArguments<ShowOptions, RemoveOptions>(args)
                .MapResult(
                    (ShowOptions opts) => RunShowAndReturnExitCode(opts),
                    (RemoveOptions opts) => RunRemoveAndReturnExitCode(opts),
                    errs => 1);
        }

        static int RunShowAndReturnExitCode(ShowOptions opts) {
            if (File.Exists(opts.FileName)) {
                return 1;
            }
            return 0;
        }

        static int RunRemoveAndReturnExitCode(RemoveOptions opts) {
            int returnCode = 0;
            string tempFile = Path.GetTempFileName();
            if (File.Exists(opts.FileName)) {
                using (Stream inFs = File.Open(opts.FileName, FileMode.Open, FileAccess.Read)) {
                    using (Stream outFs = File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.Write)) {
                       
                    }
                }
            }
            return returnCode;
        }
    }
}
