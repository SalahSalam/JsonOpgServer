using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

class JsonServer
{
    static void Main(string[] args)
    {
        Console.WriteLine("TCP JSON Server");

        // Set up the server listener on port 8080
        TcpListener listener = new TcpListener(IPAddress.Any, 8080);
        listener.Start();
        Console.WriteLine("Server started, waiting for connections...");

        while (true)
        {
            // Accept an incoming connection
            TcpClient socket = listener.AcceptTcpClient();
            Console.WriteLine("Client connected!");

            // Handle the client in a separate task
            Task.Run(() => HandleClient(socket));
        }
    }

    static void HandleClient(TcpClient socket)
    {
        // Set up the network stream, reader, and writer
        NetworkStream ns = socket.GetStream();
        StreamReader reader = new StreamReader(ns);
        StreamWriter writer = new StreamWriter(ns) { AutoFlush = true };

        while (socket.Connected)
        {
            try
            {
                // Read the client's request (which is in JSON format)
                string jsonRequest = reader.ReadLine();
                if (jsonRequest == null) break;

                Console.WriteLine("Received JSON request: " + jsonRequest);

                // Deserialize the JSON request to extract the command and numbers
                var request = JsonSerializer.Deserialize<Request>(jsonRequest);

                // Process the request based on the method
                string responseMessage = string.Empty;
                switch (request.Method.ToLower())
                {
                    case "random":
                        Random random = new Random();
                        int randomValue = random.Next(request.Number1, request.Number2 + 1);
                        responseMessage = $"Random value between {request.Number1} and {request.Number2}: {randomValue}";
                        break;

                    case "add":
                        int sum = request.Number1 + request.Number2;
                        responseMessage = $"Sum: {sum}";
                        break;

                    case "subtract":
                        int difference = request.Number1 - request.Number2;
                        responseMessage = $"Difference: {difference}";
                        break;

                    default:
                        responseMessage = "Unknown method!";
                        break;
                }

                // Serialize the response to JSON and send it back to the client
                string jsonResponse = JsonSerializer.Serialize(new { result = responseMessage });
                writer.WriteLine(jsonResponse);
            }
            catch (Exception ex)
            {
                // Send an error message if anything goes wrong
                string errorResponse = JsonSerializer.Serialize(new { error = ex.Message });
                writer.WriteLine(errorResponse);
            }
        }

        // Close the connection with the client
        socket.Close();
    }
}

// Define a class to represent the JSON request format
class Request
{
    public string Method { get; set; }
    public int Number1 { get; set; }
    public int Number2 { get; set; }
}
