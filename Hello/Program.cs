﻿using Grpc.Core;
using MagicOnion.Server;
using System;

namespace Hello
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            RandomNumbers.Initialize(1000000, 9999999, 5000000);

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
                Ports = { new ServerPort("192.168.0.4", 12345, ServerCredentials.Insecure) }
            };

            // MagicOnion起動
            server.Start();

            // コンソールアプリが落ちないようにReadLineで待つ
            Console.ReadLine();
        }
    }
}
