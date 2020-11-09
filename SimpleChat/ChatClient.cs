using System;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ChatClient
{
    public class Client
    {
        private string userName;
        private IPAddress host;
        private const int port = 2002;

        static TcpClient client;
        static NetworkStream stream;


        public Client(string host, string userName)
        {
            this.userName = userName;
            this.host = IPAddress.Parse(host);
            client = new TcpClient();
        }

       public void Start()
        { 
            try
            {
                client.Connect(host, port); 
                stream = client.GetStream(); 

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start();
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }


        static void SendMessage()
        {
            Console.WriteLine("Введите сообщение: ");

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }

        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; 
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch(SocketException e)
                {
                    Console.WriteLine("Connection failed with code: " + e.ErrorCode + "and message: " + e.Message);
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
            Environment.Exit(0); 
        }
    }
}