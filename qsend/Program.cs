using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Messaging;
using System.Reflection;

namespace QSend {
    class Program {
        static int Main(string[] args) {

            return CommandLine.Parser.Default.ParseArguments<QSendOptions>(args)
                .MapResult(
                    RunQSendAndReturnExitCode,
                    errs => 1);
        }

        private static int RunQSendAndReturnExitCode(QSendOptions opts) {

            using (MessageQueue mq = new MessageQueue(opts.DestinationQueue)) {
                using Message msg = new Message(opts.MessageBody, new XmlMessageFormatter());
                msg.Body = opts.MessageBody;
                mq.Send(msg, opts.MessageLabel);
            }
            return 0;
        }
    }
}
