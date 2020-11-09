using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ChatServer
{
    public class Server
    {
        private static TcpListener tcpListener;
        private static IPEndPoint ipEndPoint;
        private List<ClientHandler> clients = new List<ClientHandler>(); 

        protected internal void Listen()
        {
            try
            {
                GetHost();
                tcpListener = new TcpListener(ipEndPoint);
                tcpListener.Start();
                Console.WriteLine("Server started at " + ipEndPoint.Address +  " and port" + ipEndPoint.Port);

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    Console.WriteLine("Сlient connected to " + IPAddress.Parse(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString()));

                    ClientHandler ClientHandler = new ClientHandler(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(ClientHandler.Process));
                    clientThread.Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server was crashed with code" + e.ErrorCode + "and message" + e.Message);
                StopServer();
            }
        }

        protected internal void GetHost()
        {
            int choice;
            IPHostEntry host = Dns.GetHostEntry("");
            int size = host.AddressList.Length;

            Console.WriteLine("Select the interface you are going to use: \n");
            for (int i = 0; i < size; i++)
                Console.WriteLine(i + ": " + host.AddressList[i]);

            choice = Convert.ToInt32(Console.ReadLine());

            if (choice > size || choice < size)
                ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2002);
            else
             ipEndPoint = new IPEndPoint(host.AddressList[choice], 2002);
        }

        protected internal void SendMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                    clients[i].Stream.Write(data, 0, data.Length); 
            }
        }

        protected internal void StopServer()
        {
            tcpListener.Stop();

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); 
            }

            Environment.Exit(0); 
        }

        protected internal void AddConnection(ClientHandler ClientHandler)
        {
            clients.Add(ClientHandler);
        }

        protected internal void RemoveConnection(string id)
        {
            ClientHandler client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }
    }
}