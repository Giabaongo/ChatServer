using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, ChatProtocol.Constants.DefaultChatPort);
            var clientSoceket = new Socket(
                endPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
                );

            await clientSoceket.ConnectAsync(endPoint);
            var buffer = new byte[1024];

            var r = await clientSoceket.ReceiveAsync(buffer);
            if (r == 0)
            {
                showConnectionError();
                return;
            }
            var welcomeText = Encoding.UTF8.GetString(buffer, 0, r);

            Console.WriteLine(welcomeText);
            while (true)
            {

                Console.Write("Enter your message: ");
                var msg = Console.ReadLine();

                if (string.IsNullOrEmpty(msg))
                {
                    await closeConnection(clientSoceket);
                    return;
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(msg);
                    await clientSoceket.SendAsync(bytes);
                }
            }
        }

        private static async Task closeConnection(Socket clientSoceket)
        {
            clientSoceket?.Close();
        }

        private static void showConnectionError()
        {
            Console.WriteLine("Invalid protocol");
        }
    }
}
