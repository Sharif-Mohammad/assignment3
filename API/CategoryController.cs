using CJTPWebserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public class CategoryController : IController
{
    private List<Category> categories;
    private int nextId;

    public CategoryController()
    {
        // Initialize categories
        categories = new List<Category>
        {
            new Category { Cid = 1, Name = "Beverages" },
            new Category { Cid = 2, Name = "Condiments" },
            new Category { Cid = 3, Name = "Confections" }
        };
        nextId = 4;
    }

    public List<string> GetRoutes()
    {
        return new List<string> { "/api/categories" };
    }

    public string HandleRequest(string method, string path, string body)
    {
        string[] pathSegments = path.Split('/');
        int id = pathSegments.Length == 4 ? int.Parse(pathSegments[3]) : 0;

        switch (method.ToLower())
        {
            case MethodType.Read:
                return HandleRead(id);
            case MethodType.Create:
                return HandleCreate(body);
            case MethodType.Update:
                return HandleUpdate(id, body);
            case MethodType.Delete:
                return HandleDelete(id);
            default:
                return JsonUtil.Serialize(new ResponseFormat { Status = "illegal method", Body = "Bad request: Unsupported method" });
        }
    }

    private string HandleRead(int id)
    {
        if (id == 0)
        {
            // Return all categories
            return JsonUtil.Serialize(new ResponseFormat { Status = Status.ReadSuccess, Body = JsonUtil.Serialize(categories) });
        }
        else
        {
            // Return category by ID
            var category = categories.FirstOrDefault(c => c.Cid == id);
            if (category != null)
            {
                return JsonUtil.Serialize(new ResponseFormat { Status = Status.ReadSuccess, Body = JsonUtil.Serialize(category) });
            }
            else
            {
                return JsonUtil.Serialize(new ResponseFormat { Status = Status.NotFound, Body = "Not found" });
            }
        }
    }

    private string HandleCreate(string body)
    {
        try
        {
            var requestBody = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
            if (requestBody.ContainsKey("name"))
            {
                var newCategory = new Category { Cid = nextId++, Name = requestBody["name"] };
                categories.Add(newCategory);
                return JsonUtil.Serialize(new ResponseFormat { Status = Status.CreateSuccess, Body = JsonUtil    .Serialize(newCategory) });
            }
            else
            {
                return JsonUtil.Serialize(new ResponseFormat { Status = Status.BadRequest, Body =null });
            }
        }
        catch (JsonException)
        {
            return JsonUtil.Serialize(new ResponseFormat { Status = Status.BadRequest, Body = null });
        }
    }

    private string HandleUpdate(int id, string body)
    {
        try
        {
            var requestBody = JsonUtil.Deserialize<Category>(body);
            if (requestBody != null)
            {
                var category = categories.FirstOrDefault(c => c.Cid == id);
                if (category != null)
                {
                    category.Name = requestBody.Name;
                    return JsonUtil.Serialize(new ResponseFormat { Status = Status.UpdateSuccess, Body = null });
                }
                else
                {
                    return JsonUtil.Serialize(new ResponseFormat { Status = Status.NotFound, Body = null });
                }
            }
            else
            {
                return JsonUtil.Serialize(new ResponseFormat { Status = Status.BadRequest, Body = null });
            }
        }
        catch (JsonException)
        {
            return JsonUtil.Serialize(new ResponseFormat { Status = Status.BadRequest, Body = null });
        }
    }

    private string HandleDelete(int id)
    {
        var category = categories.FirstOrDefault(c => c.Cid == id);
        if (category != null)
        {
            categories.Remove(category);
            return JsonUtil.Serialize(new ResponseFormat { Status = Status.ReadSuccess, Body = "Ok" });
        }
        else
        {
            return JsonUtil.Serialize(new ResponseFormat { Status = Status.NotFound, Body = "Not found" });
        }
    }
}
