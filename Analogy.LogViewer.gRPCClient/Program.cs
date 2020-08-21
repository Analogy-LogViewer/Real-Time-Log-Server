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
        private static  AnalogyMessageProducer producer;
        static async Task Main(string[] args)
        {
            await producer.Log("test", "this", AnalogyLogLevel.Error);
            await producer.Log("test2", "this2", AnalogyLogLevel.Event);
            Console.ReadKey();
        }

    }
}
