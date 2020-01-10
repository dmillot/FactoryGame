using Diiage2022.Factory_Games.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Diage2022.Factory_Game.Server.Services;
using System.Diagnostics;

namespace Diiage2022.Factory_Games.Server.Core
{
    public class Server
    {
        private IPAddress _IpAddress = null;
        private int _Port;
        private int _ByteSize = 1024 * 1024;
        private TcpListener _Server = null;
        private List<TcpClient> _Clients = new List<TcpClient>();
        Services _services;
        Thread threadAcceptClients;

        public Server(string ip, int port)
        {
            _services = new Services();
            _IpAddress = IPAddress.Parse(ip);
            _Port = port;
            _services = new Services();
            _services.CreateGame(1, 10, 50000);
            _services.GenerateDevelopers(5, 3000);
            _Server = new TcpListener(_IpAddress, _Port);
            _Server.Start();
            StartListener();         
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartListener()
        {
            threadAcceptClients = new Thread(AcceptClients);
            threadAcceptClients.Start();
            try
            {
                while (!_services.adminStartGame) // while admin had not started the game
                {
                    Thread.Sleep(1000);
                }
                threadAcceptClients.Interrupt();
                ManageTurn();                
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                _Server.Stop();
            }
        }

        public void AcceptClients()
        {
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                TcpClient client = _Server.AcceptTcpClient();
                Console.WriteLine("New connected client !");
                _Clients.Add(client);
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(client);
            }
        }

        public void ManageTurn()
        {
            _services.InitializeGame(_Clients);
            while(true)
            {
                List<int>player = _services.StartTurn();
                for(int i = 0; i<player.Count;i++)
                {                    
                    _services.TurnFinished = false;
                    _services.SetPlayerTurn(player[i]);
                    while(_services.TurnFinished==false)
                    { Thread.Sleep(400); }
                }
            }
        }

        public void HandleClient(object obj)
        {
            //for every new client connected
            TcpClient client = (TcpClient)obj;            
            NetworkStream stream = client.GetStream();
            bool loop = true;

            try
            {
                while (loop)
                {
                    if (client.Client.Connected == true)
                    {
                        if (SocketConnected(client.Client))
                        {
                            string response = ConvertReceivedBytesArrayToString(stream);
                            Communication communication = JsonConvert.DeserializeObject<Communication>(response);
                            
                            if (response == "")
                            {
                                _Clients.Remove(client);
                                Console.WriteLine("===========================");
                                Console.WriteLine("Client n°{0} disconnected !", Thread.CurrentThread.ManagedThreadId);
                                Console.WriteLine("===========================");
                                loop = false;
                                break;
                            }
                            else
                            {
                                WriteLog(communication);
                                int result = _services.QueryTreatment(communication);
                                if(communication.RequestType == RequestType.ADMIN_START_GAME)
                                {
                                    _Clients.Remove(client);
                                    client.Close(); // stop client after admin lanch game
                                }
                                if(result==1)
                                    _services.SendToOneClient(client, communication);                                
                            }

                            //Console.WriteLine("Client n°{1} : {0}", response, Thread.CurrentThread.ManagedThreadId);
                            //Console.WriteLine("Nombre de clients : " + _Clients.Count);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                _Clients.Remove(client);
            }
        }

        private string ConvertReceivedBytesArrayToString(NetworkStream stream)
        {
            byte[] bytes = new byte[_ByteSize];
            StringBuilder myCompleteMessage = new StringBuilder();

            do
            {
                myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(bytes, 0, stream.Read(bytes, 0, bytes.Length)));
            }
            while (stream.DataAvailable);

            return myCompleteMessage.ToString();
        }

        private bool RemoveClient(TcpClient client)
        {
            try
            {
                _Clients.Remove(client);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Remove client exception: {0}", e.ToString());
            }

            return false;
        }

        private bool SocketConnected(Socket socket)
        {
            if (socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0)
            { 
                // connection is closed
                return false;
            }

            return true;
        }

        public void WriteLog(Communication com)
        {
            Log log = new Log(DateTime.Now, com);
            string json = JsonConvert.SerializeObject(log);
            using(StreamWriter streamWriter = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "Data/logs.json"), true))
            {
                streamWriter.WriteLine(json);
            }
        }
    }
}
