using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Diiage2022.Factory_Games.Server.Core
{
    class Program
    {

        private const int PORT = 7777;        

        static void Main(string[] args)
        {
            Console.WriteLine("Ip address:");
            string IP_ADDRESS = Console.ReadLine();
            Console.WriteLine("Server Starting...");
            new Thread(delegate() { new Server(IP_ADDRESS, PORT); }).Start();
            Console.WriteLine("Server Started : " + IP_ADDRESS + ":" + PORT);
        }
    }
}
