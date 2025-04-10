using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text;

class Client
{
    private const int DEFAULT_BUFLEN = 512;
    private const string DEFAULT_PORT = "27015";



    static async Task Main()
    {
        Console.Title = "CLIENT SIDE";
        Console.OutputEncoding = Encoding.UTF8;
        var ipAddress = IPAddress.Loopback;
        var remoteEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT));
        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await clientSocket.ConnectAsync(remoteEndPoint);
            Console.WriteLine("Подключение к серверу установлено.");
        }
        catch (SocketException)
        {
            Console.WriteLine("Сервер не найден. Запускаем сервер...");

            try
            {
                
                Process.Start("C:\\Users\\USER\\Desktop\\Новая папка (2)\\Server\\bin\\Debug\\net9.0\\Server.exe");
                await Task.Delay(2000);

                await clientSocket.ConnectAsync(remoteEndPoint);
                Console.WriteLine("Повторное подключение к серверу установлено");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при запуске сервера: {ex.Message}");
                return;
            }
        }

        while (true)
        {
            await Task.Delay(2000);
            Console.Write("Напишите запрос к серверу 1-Дата 2-времяы (или наберите exit для выхода): ");
            var message = Console.ReadLine();
            if (message == "exit")
            {

                clientSocket.Shutdown(SocketShutdown.Send);
                clientSocket.Close();
                Console.WriteLine("Соединение с сервером закрыто");
                break;
            }
            else
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                await clientSocket.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);


                var buffer = new byte[DEFAULT_BUFLEN];
                int bytesReceived = clientSocket.Receive(buffer);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            }
        }


    }
}
