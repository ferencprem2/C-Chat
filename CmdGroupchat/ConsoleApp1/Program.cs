using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp1;

class Program
{
    static TcpClient tcpClient;
    static TcpListener tcpListener;
    
    static void Main(string[] args)
    {
        tcpListener = new TcpListener(IPAddress.Any, 1010);
        tcpListener.Start();
        Console.WriteLine("Server started on port: 42069");
        
        tcpClient = tcpListener.AcceptTcpClient();
        Console.WriteLine("Client Connected");

        Thread recieveThread = new Thread(RecieveMessage);
        Thread sendThread = new Thread(SendMessage);
        
        recieveThread.Start();
        sendThread.Start();
    }

    static void RecieveMessage()
    {
        NetworkStream stream = tcpClient.GetStream();
        byte[] buffer = new byte[1024];
        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"{message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Client Disconnected");
                throw;
            }
        }
    }

    static void SendMessage()
    {
        NetworkStream stream = tcpClient.GetStream();
        while (true)
        {
            string message = Console.ReadLine();
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            Console.WriteLine($"Server: {message}");
            stream.Write(buffer, 0, buffer.Length);
        }
    }

}