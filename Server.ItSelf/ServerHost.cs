using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server.ItSelf
{
    public class ServerHost
    {
        private readonly IHandler _handler;

        public ServerHost(IHandler handler)
        {
            _handler = handler;
        }

        public void StartV1()
        {
            Console.WriteLine("Server Started V1");
            TcpListener listner = new TcpListener(IPAddress.Any, 80);
            listner.Start();

            while (true)
            {
                try
                {
                    using (var client = listner.AcceptTcpClient())
                    using (var stream = client.GetStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var firstLine = reader.ReadLine();
                        for (string line = null; line != string.Empty; line = reader.ReadLine())
                            ;

                        var request = RequestParser.Parse(firstLine);

                        _handler.Handle(stream, request);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void StartV2()
        {
            Console.WriteLine("Server Started V2");
            TcpListener listner = new TcpListener(IPAddress.Any, 80);
            listner.Start();

            while (true)
            {
                var client = listner.AcceptTcpClient();
                ProcessClient(client);
            }
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Server Started Async");
            TcpListener listner = new TcpListener(IPAddress.Any, 80);
            listner.Start();

            while (true)
            {
                var client = await listner.AcceptTcpClientAsync();
                var _ = ProcessClientAsync(client);
            }
        }

        private void ProcessClient(TcpClient client)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    using (client)
                    using (var stream = client.GetStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var firstLine = reader.ReadLine();
                        for (string line = null; line != string.Empty; line = reader.ReadLine())
                            ;

                        var request = RequestParser.Parse(firstLine);

                        _handler.Handle(stream, request);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (var stream = client.GetStream())
                using (var reader = new StreamReader(stream))
                {
                    var firstLine = await reader.ReadLineAsync();
                    for (string line = null; line != string.Empty; line = await reader.ReadLineAsync())
                        ;

                    var request = RequestParser.Parse(firstLine);

                    await _handler.HandleAsync(stream, request);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
