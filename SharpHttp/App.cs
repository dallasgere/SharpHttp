using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SharpHttp;

public class App
{
    public void Run()
    {
        // Define the server host and port
        string serverHost = "0.0.0.0";
        int serverPort = 8080;

        // Create a TCP listener
        TcpListener listener = new TcpListener(IPAddress.Parse(serverHost), serverPort);
        listener.Start();
        Console.WriteLine($"Listening on port: {serverPort}");

        while (true)
        {
            // Accept client connections
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream networkStream = client.GetStream();

            // Read the client's request
            byte[] requestBuffer = new byte[1024];
            int bytesRead = networkStream.Read(requestBuffer, 0, requestBuffer.Length);
            string request = Encoding.ASCII.GetString(requestBuffer, 0, bytesRead);

            Console.WriteLine($"Request: {request}");

            // Parse HTTP headers
            string[] headers = request.Split('\n');
            string[] requestLine = headers[0].Split(' ');
            string path = requestLine[1];

            Console.WriteLine($"File Name: {path}");

            // Get contents of the file
            path = "Views" + path;
            // if (path == "/")
            // {
            //     path = "index.html";
            // }
            // else if (path == "/lorem")
            // {
            //     path = "lorem.html";
            // }

            string response = string.Empty;

            try
            {
                string content = File.ReadAllText(path);
                response = "HTTP/1.0 200 OK\r\n\r\n" + content;
            }
            catch (FileNotFoundException)
            {
                response = "HTTP/1.0 404 NOT FOUND\r\n\r\nFile Not Found";
            }

            // Send HTTP response to the client
            byte[] responseBuffer = Encoding.ASCII.GetBytes(response);
            networkStream.Write(responseBuffer, 0, responseBuffer.Length);

            // Close the client connection
            client.Close();
        }

        // Stop listening and close the listener (usually not reached in a real server)
        listener.Stop();
    }
}
