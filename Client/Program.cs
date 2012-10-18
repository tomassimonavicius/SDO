using System;

namespace Client
{
    class Program
    {
        static void Main()
        {
            Client client = new Client();
            client.Connect();

            Console.ReadKey();
        }
    }
}
