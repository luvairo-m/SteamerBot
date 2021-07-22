using System;

namespace Steamer
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
            Console.ReadLine();
        }
    }
}
