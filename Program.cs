using System;

namespace BackupServiceDaemon
{
    class Program
    {
        static void Main(string[] args) {
            Application.TryConnect();

            while (true) {
                ConsoleKey info = Console.ReadKey().Key;

                if (info == ConsoleKey.NumPad1)
                    Application.Register();
                if (info == ConsoleKey.NumPad2)
                    Application.Self();
            }
        }
    }
}
