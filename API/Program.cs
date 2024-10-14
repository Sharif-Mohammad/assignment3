using CJTPWebserver;

class Program
{
    static void Main(string[] args)
    {
        CJTPService server = new CJTPService(new CategoryController());
        server.Start();
    }
}