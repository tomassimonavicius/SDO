using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            new Client();

            Console.ReadKey();
        }
    }
}
