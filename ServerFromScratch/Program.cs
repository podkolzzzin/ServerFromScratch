using Server.ItSelf;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ServerFromScratch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ServerHost host = new ServerHost(new ControllersHandler(typeof(Program).Assembly));
            
            host.StartV2();
        }
    }
}
