using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using BackupServiceDaemon.Models;
using BackupServiceDaemon.BackupAlgorithms;

namespace BackupServiceDaemon
{
    public class Application
    {
        public static bool Exit { get; set; } = false;
        static CancellationTokenSource source = new CancellationTokenSource();
        static CancellationToken token = source.Token;
        static Application() {
            try {
                SettingsService.Load();
                Login();
            }
            catch (System.IO.FileNotFoundException) {
                Console.Write("No settings found.\nPlease enter your server's address: ");
                SettingsService.Settings.Server = Console.ReadLine().Trim();
                if (SettingsService.Settings.Server[SettingsService.Settings.Server.Length - 1] != '/')
                    SettingsService.Settings.Server += '/';
                SettingsService.Save();
            }
        }
        public static void Tick() {
            Jobs();
        }

        public static void Loop() {
            while (!Exit) {
                ConsoleKey info = Console.ReadKey().Key;
                Console.WriteLine();

                /*
                if (info == ConsoleKey.F1)
                    RunBackup(FullBackup());
                if (info == ConsoleKey.F2)
                    RunBackup(DifferentialBackup());
                if (info == ConsoleKey.F3)
                    RunBackup(IncrementalBackup());
                */
                 
                
                if (info == ConsoleKey.F1)
                    Application.Register();
                if (info == ConsoleKey.F2)
                    Application.Self();
                if (info == ConsoleKey.F3) {
                    Application.Jobs();
                    SetJobs();
                }
                if (info == ConsoleKey.F4)
                    RunJob(SettingsService.Settings.Jobs[0]);
                if (info == ConsoleKey.F5)
                    SettingsService.Wipe();
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

        public static void Jobs() {
            try {
                Job[] jobs = APIService.GetJobs();
                SettingsService.Settings.Jobs = jobs;
            }
            catch  (Exception e) {
                throw e;
            }
            finally {
                SettingsService.Save();
                System.Console.WriteLine("Got {0} jobs", SettingsService.Settings.Jobs.Length);
            }
        }
        public static void SetJobs() {
            TimeSpan ts;
            source.Cancel();
            token = source.Token;
            foreach (var job in SettingsService.Settings.Jobs) {
                foreach (var time in job.Schedule) {                    
                    if (time > DateTime.Now) {
                        ts = time - DateTime.Now;
                        Task.Delay(ts, token).ContinueWith(t => RunJob(job), token);
                        System.Console.WriteLine(DateTime.Now);
                        System.Console.WriteLine(ts);
                        System.Console.WriteLine(time);
                    }
                }
            }
        }
        public static void RunJob(Job job) {
            foreach(Path source in job.Sources) {
                foreach(Path target in job.Targets) {
                    System.Console.WriteLine("{0} {1} {2}", job.Type, source.Directory, target.Directory);
                    RunBackup(FullBackup(source.Directory, target.Directory, job.Retention));
                    /*
                    if (job.Type == BackupType.Full)
                        RunBackup(FullBackup(source.Directory, target.Directory, job.Retention));
                    else if (job.Type == BackupType.Differential)
                        RunBackup(DifferentialBackup(source.Directory, target.Directory, job.Retention));
                    else if (job.Type == BackupType.Incremental)
                        RunBackup(IncrementalBackup(source.Directory, target.Directory, job.Retention));*/
                }
            }
        }
        public static void RunBackup(IBackup backup) {
            var progress = new Progress<BackupProgress>();
            progress.ProgressChanged += ( s, e ) => System.Console.WriteLine(e.Percentage);

            Task.Factory.StartNew(() => backup.Run(progress));
        }
        public static IBackup FullBackup(string source, string target, int retention) {
            return new FullBackup() { Source = source, Target = target };
        }
        public static IBackup DifferentialBackup(string source, string target, int retention) {
            return new DifferentialBackup() { Source = source, Target = target, Retention = retention };
        }
        public static IBackup IncrementalBackup(string source, string target, int retention) {
            return new IncrementalBackup() { Source = source, Target = target, Retention = retention };
        }
    }
}