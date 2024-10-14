using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJTPWebserver;

public class Router
{
    private Dictionary<string, IController> routeTable;

    public Router()
    {
        routeTable = new Dictionary<string, IController>(StringComparer.OrdinalIgnoreCase);
    }

    // Registers a controller and its routes
    public void RegisterController(IController controller)
    {
        foreach (var route in controller.GetRoutes())
        {
            if (!routeTable.ContainsKey(route))
            {
                routeTable.Add(route, controller);
            }
            else
            {
                throw new Exception($"Route {route} is already registered.");
            }
        }
    }

    // Finds the controller based on the request path
    public IController ResolveController(string path, out string resolvedPath)
    {
        // Exact match
        if (routeTable.ContainsKey(path))
        {
            resolvedPath = path;
            return routeTable[path];
        }

        // Check for parameterized routes (e.g., /api/categories/1)
        foreach (var route in routeTable.Keys)
        {
            if (path.StartsWith(route + "/", StringComparison.OrdinalIgnoreCase))
            {
                resolvedPath = route;
                return routeTable[route];
            }
        }

        resolvedPath = null;
        return null;
    }
}
