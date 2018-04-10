using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server is started");
            Server reader = new Server(808);
            reader.Run();
            Console.ReadKey();
        }
    }
}
