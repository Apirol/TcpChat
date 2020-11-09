using System;
using ChatClient;

namespace SimpleChat
{
    class Programm
    {
        static void Main()
        {
            Console.Write("Введите свое имя: ");
            string username = Console.ReadLine();
            Console.Write("Введите IP адрес сервера: ");
            string ip = Console.ReadLine();

            Client client = new Client(ip, username);
            client.Start();
        }
    }
}
