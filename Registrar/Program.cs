using System;
using Grpc.Core;
using Pericles.CommonUtils;

namespace Pericles.Registrar
{
    public static class Program
    {
        private const int Port = 50083;

        public static void Main(string[] args)
        {
            var ipAddress = IpAddressProvider.GetLocalIpAddress();
            var registrarService = new RegistrarService();
            var server = new Server
            {
                Services = { Protocol.Registrar.BindService(registrarService) },
                Ports = { new ServerPort(ipAddress, Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Registrar listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
