using CommandLine;

namespace QSend {
    public class QSendOptions {
        [Option('d', "Destination", HelpText = "Destination Queue", Required = true)]
        public string DestinationQueue { get; set; }
        [Option('t',"Transactional",HelpText="Queue is transactional",Default=true)]
        public bool Transactional { get; set; }
        [Option('l',"Label",HelpText="Message Label",Required = true)]
        public string MessageLabel { get; set; }
        [Option('b',"Body",HelpText="Message XML Body")]
        public string MessageBody { get; set; }
    }
}
