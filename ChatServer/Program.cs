using System;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            CServer server = new CServer();
            server.Start();

            Console.ReadLine();

        }
    }
}
