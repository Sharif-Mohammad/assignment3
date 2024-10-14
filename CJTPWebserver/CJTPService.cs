using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
namespace CJTPWebserver;

public class CJTPService
{
    private TcpListener listener;
    private Router router;

    public CJTPService(IController controller)
    {
        listener = new TcpListener(IPAddress.Any, 5000);
        router = new Router();

        // Register controllers
        RegisterControllers(controller);
    }

    private void RegisterControllers(IController controller)
    {

        // Register controllers with the router
        router.RegisterController(controller);

    }

    public void Start()
    {
        listener.Start();
        Console.WriteLine("Server listening on port 5000...");

        while (true)
        {
            try
            {
                TcpClient client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
                // The server should continue
            }
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                string requestString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received request: {requestString}");

                try
                {
                    var request = JsonUtil.Deserialize<RequestFormat>(requestString);
                    string response = ProcessRequest(request);

                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                }
                catch (Exception e)
                {
                    string errorResponse = JsonUtil.Serialize(new ResponseFormat { Status = Status.BadRequest, Body = null });
                    byte[] responseBytes = Encoding.UTF8.GetBytes(errorResponse);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine($"Request processing error: {e.Message}");
                }
            }

            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
            // Handle client-level exceptions but keep the server alive.
        }
    }

 

    private string ProcessRequest(RequestFormat request)
    {
        // Validate mandatory fields
        List<string> validationErrors = ValidateRequest(request);
        if (validationErrors.Count > 0)
        {
            string errorBody = string.Join(", ", validationErrors);
            return JsonUtil.Serialize(new ResponseFormat { Status = errorBody , Body = null });
        }

        // Handle 'echo' method
        if (request.Method.Equals(MethodType.Echo, StringComparison.OrdinalIgnoreCase))
        {
            return JsonUtil.Serialize(new ResponseFormat { Status = Status.ReadSuccess, Body = request.Body });
        }

        // Resolve controller based on path
        string resolvedRoute;
        IController controller = router.ResolveController(request.Path, out resolvedRoute);

        if (controller == null)
        {
            return JsonUtil.Serialize(new ResponseFormat { Status = Status.NotFound, Body = "Not found" });
        }

        // Delegate request to the controller
        return controller.HandleRequest(request.Method, request.Path, request.Body);
    }

    private List<string> ValidateRequest(RequestFormat? request)
    {
        List<string> errors = new List<string>();

        ValidateDate(request, errors);
        // Validate 'method'
        if (request == null || string.IsNullOrEmpty(request.Method))
        {
            errors.Add("missing method");
        }
        else
        {
            var validMethods = new List<string> { MethodType.Create, MethodType.Read, MethodType.Update, MethodType.Delete, MethodType.Echo };
            if (!validMethods.Contains(request.Method.ToLower()))
            {
                errors.Add("illegal method");
            }

            // Validate 'date'


            // Validate 'path'
            if (request.Method.ToLower() != MethodType.Echo)
            {
                if (string.IsNullOrEmpty(request.Path))
                {
                    errors.Add("missing resource");
                }
                else if (!request.Path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add("illegal path");
                }
                else
                {
                    if(request.Method.ToLower() == MethodType.Create)
                    {
                        if(!Utils.IsValidBasePath(request.Path))
                        {
                            errors.Add(Status.BadRequest);
                        }
                    }
                    else if (request.Method.ToLower() == MethodType.Update || request.Method.ToLower() == MethodType.Delete)
                    {
                        if (!Utils.IsValidPathWithIntId(request.Path))
                        {
                            errors.Add(Status.BadRequest);
                        }
                    }
                    else if (request.Method.ToLower() == MethodType.Read)
                    {
                        if (! (Utils.IsValidPathWithIntId(request.Path)|| Utils.IsValidBasePath(request.Path)))
                        {
                            errors.Add(Status.BadRequest);
                        }
                    }
                }
            }

            // Validate 'body'
            if (new List<string> { MethodType.Create, MethodType.Update, MethodType.Echo }.Contains(request.Method.ToLower()))
            {
                if (string.IsNullOrEmpty(request.Body))
                {
                    errors.Add("missing body");
                }
            }

            if(MethodType.Update == request.Method.ToLower() && !string.IsNullOrEmpty(request.Body))
            {
                if (!JsonUtil.IsValidJson(request.Body))
                {
                    errors.Add("illegal body");
                }
            }
        }

        return errors;
    }

    private static void ValidateDate(RequestFormat? request, List<string> errors)
    {
        if (string.IsNullOrEmpty(request.Date))
        {
            errors.Add("missing date");
        }
        else if (!Utils.IsValidUnixTimestamp(request.Date)){
            errors.Add("illegal date");
        }
               
        
    }
}
