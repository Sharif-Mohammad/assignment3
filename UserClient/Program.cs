using System.Net.Sockets;
using System.Text.Json;
using System.Text;


Console.WriteLine("Started client");
Thread.Sleep(1000);
// Example: Reading all categories
SendRequest("read", "/api/categories", DateTimeOffset.Now.ToUnixTimeSeconds(), null);

// Example: Reading a specific category by ID
SendRequest("read", "/api/categories/1", DateTimeOffset.Now.ToUnixTimeSeconds(), null);

// Example: Creating a new category
string createBody = JsonSerializer.Serialize(new { name = "Seafood" });
SendRequest("create", "/api/categories", DateTimeOffset.Now.ToUnixTimeSeconds(), createBody);

// Example: Updating a category
string updateBody = JsonSerializer.Serialize(new { cid = 3, name = "Test" });
SendRequest("update", "/api/categories/3", DateTimeOffset.Now.ToUnixTimeSeconds(), updateBody);

// Example: Reading a specific category by ID
SendRequest("read", "/api/categories/3", DateTimeOffset.Now.ToUnixTimeSeconds(), null);

// Example: Deleting a category
SendRequest("delete", "/api/categories/3", DateTimeOffset.Now.ToUnixTimeSeconds(), null);

// Example: Reading all categories
SendRequest("read", "/api/categories", DateTimeOffset.Now.ToUnixTimeSeconds(), null);


// Example: Echo method (no path required)
SendRequest("echo", null, DateTimeOffset.Now.ToUnixTimeSeconds(), "This is a plain text body");

Console.ReadKey();

static void SendRequest(string method, string path, long date, string body)
{
    try
    {
        using TcpClient client = new TcpClient("localhost", 5000);
        using NetworkStream stream = client.GetStream();

        // Create the request object
        var request = new RequestFormat(method, path, date, body);

        // Serialize request to JSON
        string requestString = JsonSerializer.Serialize(request);
        byte[] data = Encoding.UTF8.GetBytes(requestString);

        // Send the request
        stream.Write(data, 0, data.Length);

        // Receive the response
        byte[] buffer = new byte[2048];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        Console.WriteLine($"Response: {response}");

        // Close connections
        stream.Close();
        client.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

public record RequestFormat(string Method, string Path, long Date, string Body);
