using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.IO;
using System.Net.Mime;
using jpgUtility;
using JpgUtility;

namespace DeExif {
    class Program {
        static int Main(string[] args) {

      

            return CommandLine.Parser.Default.ParseArguments<ShowOptions, RemoveOptions>(args)
                .MapResult(
                    (ShowOptions opts) => RunShowAndReturnExitCode(opts),
                    (RemoveOptions opts) => RunRemoveAndReturnExitCode(opts),
                    errs => 1);
        }

        static int RunShowAndReturnExitCode(ShowOptions opts) {
            if (File.Exists(opts.FileName)) {
                foreach (string s in GetProperties(opts.FileName)) {
                    Console.WriteLine(s);
                }
            }

            return 0;
        }

        private static string[] GetProperties(string inputFile) {
            using (var reader = new ExifReader(inputFile)) {
                var props = Enum.GetValues(typeof(ExifTags)).Cast<ushort>().Select(tagID => {
                    if (reader.GetTagValue(tagID, out object val)) {
                        // Special case - some doubles are encoded as TIFF rationals. These
                        // items can be retrieved as 2 element arrays of {numerator, denominator}
                        if (val is double) {
                            if (reader.GetTagValue(tagID, out int[] rational))
                                val = $"{val} ({rational[0]}/{rational[1]})";

                            return $"{Enum.GetName(typeof(ExifTags), tagID)}: {RenderTag(val)}";
                        }

                        return $"{Enum.GetName(typeof(ExifTags), tagID)}: {RenderTag(val)}";
                    }

                    return null;

                }).Where(x => x != null).ToArray();
                return props;
            }
        }
        private static string RenderTag(object tagValue) {
            // Arrays don't render well without assistance.
            if (tagValue is Array array) {
                // Hex rendering for really big byte arrays (ugly otherwise)
                if (array.Length > 20 && array.GetType().GetElementType() == typeof(byte))
                    return "0x" + string.Join("", array.Cast<byte>().Select(x => x.ToString("X2")).ToArray());

                return string.Join(", ", array.Cast<object>().Select(x => x.ToString()).ToArray());
            }

            return tagValue.ToString();
        }

        static int RunRemoveAndReturnExitCode(RemoveOptions opts) {
            int returnCode = 0;

            if (!File.Exists(opts.FileName)) {
                return returnCode;
            }
            string tempFile = Path.GetTempFileName();
            string origFile = Path.GetFullPath(opts.FileName);
            using (Stream inFs = File.Open(origFile, FileMode.Open, FileAccess.Read)) {
                using (Stream outFs = File.Open(tempFile, FileMode.OpenOrCreate, FileAccess.Write)) {
                    ExifRemover.PatchAwayExif(inFs, outFs);
                    outFs.Close();
                }
                inFs.Close();
            }

            string origDir = Path.GetDirectoryName(origFile);
            string newFile = $"NEW{Path.GetFileName(origFile)}";

            File.Move(tempFile, Path.Combine(origDir,newFile));
            return returnCode;
        }
    }
}
