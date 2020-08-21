using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Analogy.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;


namespace Analogy.LogViewer.gRPCClient
{
    class Program
    {
        private static AnalogyMessageProducer producer;
        static async Task Main(string[] args)
        {
            producer=new AnalogyMessageProducer();
            for (int i = 0; i < 10000; i++)
            {
                await producer.Log("test:" + i, "this", AnalogyLogLevel.Error);
                await producer.Log("test2:" + i, "this2", AnalogyLogLevel.Event);
                await Task.Delay(500);
            }

            Console.ReadKey();
        }

    }
}
