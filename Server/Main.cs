using System;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static Server server; 
        static Thread listenThread; // поток для прослушивания
        static void Main(string[] args)
        {
            try
            {
                server = new Server();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); 
            }
            catch (Exception ex)
            {
                server.StopServer();
                Console.WriteLine(ex.Message);
            }
        }
    }
}