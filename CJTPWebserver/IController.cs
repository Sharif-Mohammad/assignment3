using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJTPWebserver
{
    public interface IController
    {
        // Returns a list of routes that the controller handles
        List<string> GetRoutes();

        // Handles the incoming request and returns a response
        string HandleRequest(string method, string path, string body);
    }

}
