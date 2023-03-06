using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

Socket s = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
IPAddress ip = IPAddress.Parse("224.5.6.7");
Console.Write("Enter your name: ");
string? username = Console.ReadLine();

Task.Run(ReceiveMessageAsync);
await SendMessageAsync();

// отправка сообщений в группу
async Task SendMessageAsync()
{
    s.SetSocketOption(SocketOptionLevel.IP,
            SocketOptionName.MulticastTimeToLive, 2);
    while (true)
    {
        string? message = Console.ReadLine(); // сообщение для отправки
        // если введена пустая строка, выходим из цикла и завершаем ввод сообщений
        if (string.IsNullOrWhiteSpace(message)) break;
        // иначе добавляем к сообщению имя пользователя
        message = $"{username}: {message}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        // и отправляем в группу
        IPEndPoint ipep = new IPEndPoint(ip, 4567);
        s.Connect(ipep);
        s.Send(data, data.Length, SocketFlags.None);

        s.Close();
    }
}
// получение сообщений из группы
async Task ReceiveMessageAsync()
{
    while (true)
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
        ProtocolType.Udp);
        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 4567);
        s.Bind(ipep);
        IPAddress ip = IPAddress.Parse("224.5.6.7");

        s.SetSocketOption(SocketOptionLevel.IP,
            SocketOptionName.AddMembership,
                new MulticastOption(ip, IPAddress.Any));
        byte[] b = new byte[1024];
        s.Receive(b);
        string message = Encoding.UTF8.GetString(b.Buffer);
        Console.WriteLine(message);
    }
}