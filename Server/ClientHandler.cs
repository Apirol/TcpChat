using System;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class ClientHandler
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }


        private string userName;
        private TcpClient client;
        private Server server; 

        public ClientHandler(TcpClient tcpClient, Server serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();

                server.SendMessage("Введите ваше имя", this.Id);
                userName = GetMessage();

                string message = userName + " вошел в чат";

                server.SendMessage(message, this.Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", userName, message);
                        Console.WriteLine(message);
                        server.SendMessage(message, this.Id);
                    }
                    catch
                    {
                        message = String.Format("{0}: покинул чат", userName);
                        Console.WriteLine(message);
                        server.SendMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Client handler was down with error: " + e.ErrorCode + "and message: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Client handler was down with message: " + e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        private string GetMessage()
        {
            byte[] data = new byte[256];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            try
            {
                if (Stream != null)
                    Stream.Close();
                if (client != null)
                    client.Close();
            }
            catch
            {
                throw new Exception("Cannot close ClientHandler");
            }
        }
    }
}