using System.Net; // основное пространство имён для работы с сетевыми адресами и протоколами
using System.Net.Sockets; // пространство имён для работы с сокетами
using System.Text; // пространство имён для работы с кодировками

class Server // класс реализует серверную логику
{
    private const int DEFAULT_BUFLEN = 512; // задаёт размер буфера для получения данных
    // если нужно работать с большим количеством данных, рекомендуется использовать буферы от 4 КБ до 64 КБ (размер, с которым обычно работают сетевые приложения)
    // если данные небольшие и ожидается, что они будут приходить в небольших объёмах, можно использовать буфер 512 байт или даже меньше
    private const string DEFAULT_PORT = "27015"; // указывает порт, на котором сервер будет прослушивать подключения
    private const int PAUSE = 1000; // задаёт паузу в миллисекундах для красоты и удобства вывода сообщений (можно смело убрать)

    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8; // кириллица
        Console.Title = "SERVER SIDE";
        Console.WriteLine("Процесс сервера запущен!");
        await Task.Delay(PAUSE);

        try
        {
            var ipAddress = IPAddress.Any; // получает любой доступный IP-адрес для прослушивания (означает, что сервер будет слушать на всех интерфейсах, например, Wi-Fi, Ethernet
            var localEndPoint = new IPEndPoint(ipAddress, int.Parse(DEFAULT_PORT)); // создаёт конечную точку (адрес и порт), к которой сервер будет привязан

            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // создаёт сокет для использования TCP-соединения (потоковый сокет)
            listener.Bind(localEndPoint); // привязывает сокет к указанному адресу и порту

            Console.WriteLine("Получение адреса и порта сервера прошло успешно!");
            await Task.Delay(PAUSE);

            listener.Listen(10); // начинает прослушивание входящих соединений, устанавливая максимальное количество ожидающих соединений (10), то есть сервер может иметь до 10 клиентов (соединений) в очереди на подключение
            Console.WriteLine("Начинается прослушивание информации от клиента.\nПожалуйста, запустите клиентскую программу!");

            var clientSocket = await listener.AcceptAsync(); // ожидает подключение клиента и принимает его, возвращая сокет для общения с клиентом. есть AcceptAsync(), чтоб не блокировать поток
            Console.WriteLine("\nПодключение с клиентской программой установлено успешно!");

            listener.Close(); // закрывает сокет слушателя, так как соединение с клиентом уже установлено
            // соединение с клиентом теперь управляется отдельным сокетом, полученным от метода Accept(), и слушающий сокет больше не нужен

            var buffer = new byte[DEFAULT_BUFLEN]; // создаёт буфер для хранения полученных данных
            while (true)
            {

                int bytesReceived = await clientSocket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                if (bytesReceived > 0) // если данные были получены
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine($"Процесс клиента отправил сообщение: {message}"); // выводит полученное сообщение

                    string response = "";






                    //////////////////
                    if (message == "1")
                    {
                        response = $"Текущая дата: {DateTime.Now:dd.MM.yyyy}";
                    }
                    else if (message == "2")
                    {
                        response = $"Текущее время: {DateTime.Now:HH:mm:ss}";
                    }
                    else
                    {
                        response = "Ошибка: введите '1' или '2'";
                    }

                    ////////////////

                    byte[] responseBytes = Encoding.UTF8.GetBytes(response); // преобразует ответ в массив байтов
                    await clientSocket.SendAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None);

                    Console.WriteLine($"Процесс сервера отправляет ответ: {response}");
                }
                else if (bytesReceived == 0) // если клиент закрыл соединение (получен 0 байтов)
                {
                    Console.WriteLine("Соединение закрывается..."); // информирует о том, что соединение будет закрыто
                    break;
                }
            }

            clientSocket.Shutdown(SocketShutdown.Send); // закрывает сокет для отправки данных (клиент завершил отправку)
            clientSocket.Close(); // закрывает сокет для общения с клиентом
            Console.WriteLine("Процесс сервера завершает свою работу!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}
