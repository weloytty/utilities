using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.IO;
using System.Linq.Expressions;
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
                try {
                    foreach (var s in GetProperties(opts.FileName)) {
                        Console.WriteLine($"{s.Key.PadRight(25)}:{s.Value.PadLeft(55)}");
                    }
                } catch (Exception e) {
                    Console.WriteLine($"Error procecssing {opts.FileName}:{e.Message}");
                }
            }

            return 0;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetProperties(string inputFile) {

          
                using (var reader = new ExifReader(inputFile)) {
                    foreach (var tagId in Enum.GetValues(typeof(ExifTags)).Cast<ushort>()) {
                        if (reader.GetTagValue(tagId, out object val)) {
                            if (val is double) {
                                if (reader.GetTagValue(tagId, out int[] rational)) {
                                    val = $"{val} ({rational[0] / rational[1]})";
                                }
                            }

                            yield return new KeyValuePair<string, string>($"{Enum.GetName(typeof(ExifTags), tagId)}",
                                $"{RenderTag(val)}");
                        }
                    }

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
                    try {
                        ExifRemover.PatchAwayExif(inFs, outFs);
                    } catch (ExifLibException xifLibEx) {
                        Console.WriteLine($"Exception {xifLibEx.Message} trying to read '{opts.FileName}'");
                    } finally {
                        outFs.Close();
                    }
                }
                inFs.Close();
            }

            string origDir = Path.GetDirectoryName(origFile);
            string newFile = $"NEW{Path.GetFileName(origFile)}";
            string newFullPath = Path.Combine(origDir, newFile);
            Console.WriteLine($"SOURCE: {opts.FileName}");
            Console.WriteLine($"TEMP  : {tempFile}");
            Console.WriteLine($"DEST  : {newFullPath}");
            try {
                if (File.Exists(newFullPath)) { File.Delete(newFullPath); }
                File.Move(tempFile, newFullPath);
                Console.WriteLine($"DONE.");
            } catch (Exception e) {
                Console.WriteLine($"ERROR COPYING. Cleaned file: {tempFile}");
            }


            return returnCode;
        }
    }
}
