// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;

Console.WriteLine("TCP Server OblOpg4");

TcpListener listener = new TcpListener(IPAddress.Any, 7);

listener.Start();
while (true)
{
    TcpClient socket = listener.AcceptTcpClient();
    IPEndPoint clientEndPoint = socket.Client.RemoteEndPoint as IPEndPoint;
    Console.WriteLine("Client connected: " + clientEndPoint.Address);

    Task.Run(() => HandleClient(socket));
}

// listener.Stop();

// Method to handle client connections
void HandleClient(TcpClient socket)
{
    NetworkStream ns = socket.GetStream();
    StreamReader reader = new StreamReader(ns);
    StreamWriter writer = new StreamWriter(ns);

    int counter = 0;

    while (socket.Connected)
    {
        string message = reader.ReadLine().ToLower(); // Modtager besked fra client (i lowercase)
        Console.WriteLine("Besked modtaget: " + message);

        counter++;

        switch (message)
        {
            case "stop":
                HandleStop(writer, socket);
                break;

            case "random":
                HandleRandom(reader, writer);
                break;

            case "add":
                HandleAdd(reader, writer);
                break;

            case "subtract":
                HandleSubtract(reader, writer);
                break;

            default:
                writer.WriteLine("Unknown command");
                writer.Flush();
                break;
        }
    }
}
// Method to handle the "stop" command
void HandleStop(StreamWriter writer, TcpClient socket)
{
    writer.WriteLine("Goodbye: Closing down Client");
    writer.Flush();
    Console.WriteLine("Client connection closed.");
    socket.Close();
}

// Method to handle the "random" command
void HandleRandom(StreamReader reader, StreamWriter writer)
{
    writer.WriteLine("Enter two random numbers: x y");
    writer.Flush();

    string[] messageA = reader.ReadLine().Split(" ");
    int x = Int32.Parse(messageA[0]);
    int y = Int32.Parse(messageA[1]);

    int minValue = Math.Min(x, y);
    int maxValue = Math.Max(x, y);

    Random random = new Random();
    int n = random.Next(minValue, maxValue + 1); // +1 for at inkludere maxValue i det tilfældige interval: Random.Next()
    string sn = n.ToString();

    // Logging the operation and result to the console
    Console.WriteLine($"Tilfældig nummer mellem {x} og {y} er: {n}");
    Console.WriteLine($"resultatet sendt: {sn}");

    // Sending the result back to the client
    writer.WriteLine(sn);
    writer.Flush();
}

// Method to handle the "add" command
void HandleAdd(StreamReader reader, StreamWriter writer)
{
    writer.WriteLine("Add two numbers: E.g: 1 2");
    writer.Flush();

    string[] messageB = reader.ReadLine().Split(" ");
    int a = Int32.Parse(messageB[0]);
    int b = Int32.Parse(messageB[1]);

    int sum = a + b;
    string samletSum = sum.ToString();

    // Logging the operation and result to the console
    Console.WriteLine($"{a} + {b} = {sum}");
    Console.WriteLine($"resultatet sendt: {samletSum}");

    // Sending the result back to the client
    writer.WriteLine(samletSum);
    writer.Flush();
}

// Method to handle the "subtract" command
void HandleSubtract(StreamReader reader, StreamWriter writer)
{
    writer.WriteLine("Subtract two numbers: E.g: 5 2");
    writer.Flush();

    string[] messageC = reader.ReadLine().Split(" ");
    int c = Int32.Parse(messageC[0]);
    int d = Int32.Parse(messageC[1]);

    int forskel = c - d;
    string samletForskel = forskel.ToString();

    // Logging the operation and result to the console
    Console.WriteLine($"{c} - {d} = {forskel}");
    Console.WriteLine($"resultatet sendt: {samletForskel}");

    // Sending the result back to the client
    writer.WriteLine(samletForskel);
    writer.Flush();
}