using Grpc.Core;
using MagicOnion.Server;
using System;

namespace Hello
{
    class Program
    {
        static void Main(string[] args)
        {
            //コンソールにログを表示させる
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            var service = MagicOnionEngine.BuildServerServiceDefinition(isReturnExceptionStackTraceInErrorDetail: true);

            // localhost:12345でListen
            ChannelOption[] options =
                {
                    new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
                    new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue)
                };
            var server = new global::Grpc.Core.Server(options)
            {
                Services = { service },
                Ports = { new ServerPort("10.128.0.16", 12345, ServerCredentials.Insecure) }
            };

            // MagicOnion起動
            server.Start();

            // コンソールアプリが落ちないようにReadLineで待つ
            Console.ReadLine();
        }
    }
}
