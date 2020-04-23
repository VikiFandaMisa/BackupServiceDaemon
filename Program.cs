using System;
using System.Threading;

namespace BackupServiceDaemon
{
    class Program
    {
        public static Application app = new Application();
        static void Main(string[] args)
        {
            Console.Read();
            
            //Timer timer = new Timer(Tick, null, 0, 5000);

            while (true)
            {
                ConsoleKey info = Console.ReadKey().Key;

                if (info == ConsoleKey.NumPad1)
                    app.Init();
                if (info == ConsoleKey.NumPad2)
                    app.Init();
            }
        }
        public static void Tick(object O)
        {
            Console.WriteLine("---" + DateTime.Now + "---");
            app.Init();
            Console.WriteLine();
        }
    }
}
