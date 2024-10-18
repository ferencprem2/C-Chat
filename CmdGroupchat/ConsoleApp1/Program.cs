using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp1;

class Program
{
    static TcpListener tcpListener;
    static List<TcpClient> clients = new List<TcpClient>();
    static object lockObject = new object();
    
    static void Main(string[] args)
    {
        tcpListener = new TcpListener(IPAddress.Any, 1010);
        tcpListener.Start();
        Console.WriteLine("Server started on port: 1010");
        
        Thread acceptClientsThread = new Thread(AcceptClients);
        acceptClientsThread.Start();
    }

    static void AcceptClients()
    {
        while (true)
        {
            TcpClient newClient = tcpListener.AcceptTcpClient();
            lock (lockObject)
            {
                clients.Add(newClient);
            }
            Console.WriteLine("Client Connected");

            Thread clientThread = new Thread(() => HandleClient(newClient));
            clientThread.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Client: {message}");
                    BroadcastMessage(message, client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Client Disconnected");
                lock (lockObject)
                {
                    clients.Remove(client);
                }
                break;
            }
        }
    }

    static void BroadcastMessage(string message, TcpClient senderClient)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        lock (lockObject)
        {
            foreach (var client in clients)
            {
                if (client != senderClient)
                { 
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error sending message to a client: " + e.Message);
                    }
                }
            }
        }
    }
}