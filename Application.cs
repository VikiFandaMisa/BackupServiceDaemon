using System;
using System.Threading.Tasks;

using BackupServiceDaemon.Models;
using BackupServiceDaemon.BackupAlgorithms;

namespace BackupServiceDaemon
{
    public class Application
    {
        public static bool Exit { get; set; } = false;
        static Application() {
            /*
            try {
                SettingsService.Load();
            }
            catch (System.IO.FileNotFoundException) {
                Console.Write("No settings found.\nPlease enter your server's address: ");
                SettingsService.Settings.Server = Console.ReadLine().Trim();
                if (SettingsService.Settings.Server[SettingsService.Settings.Server.Length - 1] != '/')
                    SettingsService.Settings.Server += '/';
                SettingsService.Save();
            }
            */
        }

        public static void Loop() {
            while (!Exit) {
                ConsoleKey info = Console.ReadKey().Key;
                Console.WriteLine();

                if (info == ConsoleKey.F1)
                    RunBackup(FullBackup());
                if (info == ConsoleKey.F2)
                    RunBackup(DifferentialBackup());
                if (info == ConsoleKey.F3)
                    RunBackup(IncrementalBackup());
                 
                /*
                if (info == ConsoleKey.F1)
                    Application.Register();
                if (info == ConsoleKey.F2)
                    Application.Self();
                if (info == ConsoleKey.F3)
                    SettingsService.Wipe();
                */
            }
        }

        public static void Login() {
            if (SettingsService.Settings.ID == null) {
                Console.WriteLine("Computer is not registred yet");
                return;
            }

            try {
                APIService.Token = APIService.GetToken(new TokenRequest() {
                    ID = (int) SettingsService.Settings.ID
                });
            }
            catch  (Exception e) {
                throw e;
            }
            finally {
                System.Console.WriteLine("Successfully logged in");
            }
        }

        public static void Register() {
            try {
                Computer self = APIService.Register(new ComputerRegistration() {
                    Hostname = PCInfo.GetHostname(),
                    MAC = PCInfo.GetMAC(),
                    IP = PCInfo.GetIP()
                });
                SettingsService.Settings.ID = self.ID;
            }
            catch  (Exception e) {
                throw e;
            }
            finally {
                SettingsService.Save();
                System.Console.WriteLine("Registred successfully");
                Login();
            }
        }

        public static void Self() {
            if (SettingsService.Settings.ID == null) {
                Console.WriteLine("Computer is not registred yet");
                return;
            }

            try {
                Computer self = APIService.GetSelf();
                System.Console.WriteLine(self.ToString());
            }
            catch  (Exception e) {
                throw e;
            }
        }

        public static void RunBackup(IBackup backup) {
            var progress = new Progress<BackupProgress>();
            progress.ProgressChanged += ( s, e ) => System.Console.WriteLine(e.Percentage);

            Task.Factory.StartNew(() => backup.Run(progress));
        }

        public static IBackup FullBackup(Template template, string source, string target) {
            return new FullBackup() { Source = source, Target = target };
        }
        public static IBackup DifferentialBackup(Template template, string source, string target) {
            return new DifferentialBackup() { Source = source, Target = target, Retention = template.Retention };
        }
        public static IBackup IncrementalBackup(Template template, string source, string target) {
            return new IncrementalBackup() { Source = source, Target = target, Retention = template.Retention };
        }
    }
}