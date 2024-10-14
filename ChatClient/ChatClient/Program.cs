using System.Net.Sockets;
using System.Text;

namespace ChatClient;

class Program
{
    static TcpClient _tcpClient;
    static Stream _tcpStream;
    static void Main(string[] args)
    {

        Console.WriteLine("Enter server ip address: ");
        string serverIp = Console.ReadLine();
        
        _tcpClient = new TcpClient();
        _tcpClient.Connect(serverIp, 1010);
        Console.WriteLine("Connected to server");
        
        _tcpStream = _tcpClient.GetStream();

        Thread recieveThread = new Thread(RecieveMessage);
        Thread sendThread = new Thread(SendMessage);
        
        recieveThread.Start();
        sendThread.Start();
    }

    static void RecieveMessage()
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            try
            {
                int bytesRead = _tcpStream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Server: {message}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection closed by server");
                throw;
            }
        }
    }

    static void SendMessage()
    {
        Console.Write("Név: ");
        string name = Console.ReadLine();
    
        while (true)
        {
            Console.Write($"{name}: ");
            
            int cursorTopBeforeInput = Console.CursorTop;
            
            string message = Console.ReadLine();
            
            Console.SetCursorPosition(0, cursorTopBeforeInput); 
            Console.Write(new string(' ', Console.WindowWidth));  
            Console.SetCursorPosition(0, cursorTopBeforeInput);  
            
            Console.WriteLine($"{name}: {message}");
            
            byte[] buffer = Encoding.UTF8.GetBytes($"{name}: {message}");
            _tcpStream.Write(buffer, 0, buffer.Length);
        }
    }

}