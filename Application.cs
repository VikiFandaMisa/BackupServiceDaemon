using BackupServiceDaemon.Models;
using System;

namespace BackupServiceDaemon
{
    public class Application
    {
        static Application() {
            try {
                SettingsService.Load();
            }
            catch (System.IO.FileNotFoundException) {
                System.Console.WriteLine("No settings found.\n");
            }
        }

        public static void TryConnect() {

        }

        public static void Register() {

        }

        public static void Self() {

        }
    }
}