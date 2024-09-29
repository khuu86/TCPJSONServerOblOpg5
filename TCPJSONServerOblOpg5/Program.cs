// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text.Json;

Console.WriteLine("TCP JSON Server OblOpg5");

TcpListener listener = new TcpListener(IPAddress.Any, 6771);

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

    while (socket.Connected)
    {
        string message = reader.ReadLine(); // Modtager besked fra client (i lowercase)
        Console.WriteLine("JESON Besked modtaget: " + message);

        try
        {
            Request? request = JsonSerializer.Deserialize<Request>(message);

            if (request != null)
            { // Hvis request ikke er null, så er der modtaget en gyldig JSON besked
                Console.WriteLine($"Request modtaget: {request}");

                switch (request.Method.ToLower())
                {
                    case "stop":
                        HandleStop(writer, socket);
                        break;

                    case "random":
                        HandleRandom(request, writer);
                        break;

                    case "add":
                        HandleAdd(request, writer);
                        break;

                    case "subtract":
                        HandleSubtract(request, writer);
                        break;

                    default:
                        SendError("Unknown command", writer);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            SendError("Invalid JSON", writer);
        }
    }
    socket.Close();
}
// Method to handle the "stop" command
void HandleStop(StreamWriter writer, TcpClient socket)
{
    // Send a message to the client confirming the disconnection
    writer.WriteLine(JsonSerializer.Serialize(new { Message = "Goodbye: Closing down Client" }));
    writer.Flush();

    // Log that the client has disconnected
    Console.WriteLine("Client connection closed.");

    // Close the client socket, but do not stop the server
    socket.Close();
}

// Method to handle the "random" command
void HandleRandom(Request request, StreamWriter writer)
{
    int minValue = Math.Min(request.Tal1, request.Tal2);
    int maxValue = Math.Max(request.Tal1, request.Tal2);

    Random random = new Random();
    int n = random.Next(minValue, maxValue + 1); // +1 for at inkludere maxValue i det tilfældige interval: Random.Next()

    // Sending the result back to the client
    var response = new { Result = n };
    writer.WriteLine(JsonSerializer.Serialize(response));
    writer.Flush();
}

// Method to handle the "add" command
void HandleAdd(Request request, StreamWriter writer)
{
    int sum = request.Tal1 + request.Tal2;

    // Sending the result back in JSON format to the client
    var response = new { Result = sum };
    writer.WriteLine(JsonSerializer.Serialize(response));
    writer.Flush();
}

// Method to handle the "subtract" command
void HandleSubtract(Request request, StreamWriter writer)
{
    int forskel = request.Tal1 - request.Tal2;

    // Sending the result back in JSON format to the client
    var response = new { Result = forskel };
    writer.WriteLine(JsonSerializer.Serialize(response));
    writer.Flush();
}

// Method to send error message
void SendError(string errorMessage, StreamWriter writer)
{
    var errorResponse = new { Error = errorMessage };
    writer.WriteLine(JsonSerializer.Serialize(errorResponse));
    writer.Flush();
}

public class Request
{
    public string Method { get; set; }
    public int Tal1 { get; set; }
    public int Tal2 { get; set; }

    public Request()
    {
    }

    public Request(string method, int tal1, int tal2)
    {
        Method = method;
        Tal1 = tal1;
        Tal2 = tal2;
    }

    public override string ToString()
    {
        return $"{Method} {Tal1} {Tal2}";
    }
}