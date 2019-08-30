using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace DeExif {

    interface IExifOptions {
        [Option('q', "quiet",
            HelpText = "Suppresses summary messages.")]
        bool Quiet { get; set; }

        [Value(0, MetaName = "input file",
            HelpText = "Input file to be processed.",
            Required = true)]
        string FileName { get; set; }
    }

    [Verb("show", HelpText = "Displays a file's Exif information")]
    public class ShowOptions : IExifOptions {
        public string FileName { get; set; }
        public bool Quiet { get; set; }

        [Usage(ApplicationAlias = "deexif")]
        public static IEnumerable<Example> Examples {
            get {
                return new List<Example>() {
                    new Example("Filename to be processed", new ShowOptions {FileName = "file.jpg"})
                };
            }
        }
    }

    [Verb("remove", HelpText = "Removes Exif information from a file")]
    class RemoveOptions : IExifOptions {

        public string FileName { get; set; }
        public bool Quiet { get; set; }

        [Usage(ApplicationAlias = "deexif")]
        public static IEnumerable<Example> Examples {
            get {
                return new List<Example>() {
                    new Example("Filename to be processed", new RemoveOptions {FileName = "file.jpg"})
                };
            }
        }

    }
}
