using System;
using Grpc.Core;
using Chatter;
using GreeterServer.ServiceImplementation;

namespace GreeterServer
{
  class Program
  {
    const int Port = 50051;

    public static void Main(string[] args)
    {
      Server server = new Server
      {
        Services = { Greeter.BindService(new ChatterImpl()) },
        Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
      };
      server.Start();

      Console.WriteLine("Greeter server listening on port " + Port);
      Console.WriteLine("Press any key to stop the server...");
      Console.ReadKey();

      server.ShutdownAsync().Wait();
    }
  }
}