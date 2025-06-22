using System.Net;
using System.Net.Sockets;
using System.Text;
using ChatProtocol;

namespace ChatServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var clientId = 1;
            var endPoint = new IPEndPoint(IPAddress.Loopback, ChatProtocol.Constants.DefaultChatPort);
            var serverSocket = new Socket(
                endPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
                );

            serverSocket.Bind(endPoint);

            Console.WriteLine($"Listening... (port {endPoint.Port})");

            serverSocket.Listen();

            var clientHandlers = new List<Task>();

            while (true)
            {
                var clientSocket = await serverSocket.AcceptAsync();
                var t = handleClientRequestAsync(clientSocket, clientId++);
                clientHandlers.Add(t);
            }

            Task.WaitAll([.. clientHandlers]);
        }

        private static async Task handleClientRequestAsync(Socket clientSocket, int clientId)
        {
            Console.WriteLine($"[Client {clientId}] Connected");

            var welcomeBytes = Encoding.UTF8.GetBytes(Constants.WelComeText);
            await clientSocket.SendAsync(welcomeBytes);

            var buffer = new byte[1024];

            while (true)
            {
                var r = await clientSocket.ReceiveAsync(buffer);
                var msg = Encoding.UTF8.GetString(buffer, 0, r);

                if (msg.Equals(Constants.CommandShutdown))
                {
                    closeConnection(clientSocket);
                    Console.WriteLine($"[Client {clientId}] disconnected!");
                    break;
                }

                Console.WriteLine($"[CLient {clientId}]: {msg}");
            }
        }

        private static void closeConnection(Socket clientSocket)
        {
            clientSocket.Close();
        }
    }
}
