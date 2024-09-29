using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;

class JsonClient
{
    static void Main(string[] args)
    {
        Console.WriteLine("TCP JSON Client:");

        // Establish TCP connection to the server
        TcpClient socket = new TcpClient("127.0.0.1", 8080);  // Ensure you use the same port as the server

        // Set up the network stream, reader, and writer
        NetworkStream ns = socket.GetStream();
        StreamReader reader = new StreamReader(ns);
        StreamWriter writer = new StreamWriter(ns) { AutoFlush = true };

        bool keepSending = true;

        while (keepSending)
        {
            // Ask for the method type (Random, Add, Subtract)
            Console.WriteLine("Enter method (Random, Add, Subtract or 'close' to exit):");
            string method = Console.ReadLine();

            if (method.ToLower() == "close")
            {
                keepSending = false;
                break;
            }

            // Ask for two numbers
            Console.WriteLine("Enter the first number:");
            int number1 = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the second number:");
            int number2 = int.Parse(Console.ReadLine());

            // Create an object representing the request
            var request = new
            {
                method = method,
                number1 = number1,
                number2 = number2
            };

            // Serialize the request object to JSON
            string jsonRequest = JsonSerializer.Serialize(request);

            // Send the JSON request to the server
            writer.WriteLine(jsonRequest);

            // Read the server's response
            string jsonResponse = reader.ReadLine();
            Console.WriteLine("Response from server: " + jsonResponse);
        }

        // Close the connection
        socket.Close();
    }
}
