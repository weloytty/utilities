using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using System.Reflection;
using MSMQ.Messaging;

namespace QSend {
    class Program {
        static int Main(string[] args) {

            return CommandLine.Parser.Default.ParseArguments<QSendOptions>(args)
                .MapResult(
                    RunQSendAndReturnExitCode,
                    errs => 1);
        }

        private static int RunQSendAndReturnExitCode(QSendOptions opts) {

            using (MSMQ.Messaging.MessageQueue mq = new MessageQueue(opts.DestinationQueue)) {
                using Message msg = new Message(opts.MessageBody, new XmlMessageFormatter());
                msg.Body = opts.MessageBody;
                mq.Send(msg, opts.MessageLabel);
            }
            return 0;
        }
    }
}
