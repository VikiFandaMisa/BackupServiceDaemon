using System;
using System.Threading.Tasks;
using System.Threading;

using BackupServiceDaemon.Models;
using BackupServiceDaemon.Backuping;
using BackupServiceDaemon.Backuping.Backups;
using BackupServiceDaemon.Backuping.FileSystemAPIs;
using System.Net;

namespace BackupServiceDaemon {
    public class Application {
        public static bool Exit { get; set; } = false;
        static CancellationTokenSource Source = new CancellationTokenSource();
        static Application() {
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
            Login();
        }

        public static void Loop() {
            Self();
            while (!Exit) {
                Jobs();
                SetJobs();
                Thread.Sleep(10 * 60 * 1000);
            }
        }

        public static void Login() {
            if (SettingsService.Settings.ID == null) {
                Console.WriteLine("Computer is not registred yet");
                return;
            }

            try {
                APIService.Token = APIService.GetToken(new TokenRequest() {
                    ID = (int)SettingsService.Settings.ID
                });
            }
            catch (Exception e) {
                throw e;
            }
            finally {
                System.Console.WriteLine("Successfully logged in");
            }
        }

        public static void Register() {
            try {
                Computer self = APIService.Register(new ComputerRegistration() {
                    Hostname = ComputerInfo.GetHostname(),
                    MAC = ComputerInfo.GetMAC(),
                    IP = ComputerInfo.GetIP()
                });
                SettingsService.Settings.ID = self.ID;
            }
            catch (Exception e) {
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
                Console.WriteLine("Computer is not logged in yet");
                return;
            }

            try {
                Computer self = APIService.GetSelf();
                System.Console.WriteLine(self.ToString());
            }
            catch (Exception e) {
                throw e;
            }
        }

        public static void Jobs() {
            try {
                Job[] jobs = APIService.GetJobs();
                SettingsService.Settings.Jobs = jobs;
            }
            catch (Exception e) {
                throw e;
            }
            System.Console.WriteLine("Got " + SettingsService.Settings.Jobs.Length + " jobs");
            SettingsService.Save();
        }
        public static void SetJobs() {
            TimeSpan ts;
            Source.Cancel();
            foreach (var job in SettingsService.Settings.Jobs) {
                foreach (var time in job.Schedule) {
                    if (time > DateTime.Now) {
                        ts = time - DateTime.Now;
                        Task.Delay(ts, Source.Token).ContinueWith(t => RunJob(job), Source.Token);
                    }
                }
            }
        }
        public static void RunJob(Job job) {
            foreach (Path target in job.Targets) {
                foreach (Path source in job.Sources) {
                    IFileSystemAPI fileSystemAPI;
                    if (target.Network == null && job.TargetFileType == BackupFileType.Zip) {
                        fileSystemAPI = new LocalZIPFileSystemAPI();
                    }
                    else if (target.Network != null && job.TargetFileType == BackupFileType.Plain) {
                        fileSystemAPI = new FTPFileSystemAPI() {
                            Server = target.Network.Server,
                            Creds = new NetworkCredential(target.Network.Name, target.Network.Password)
                        };
                    }
                    else if (target.Network != null && job.TargetFileType == BackupFileType.Zip) {
                        fileSystemAPI = new FTPZIPFileSystemAPI() {
                            Server = target.Network.Server,
                            Creds = new NetworkCredential(target.Network.Name, target.Network.Password)
                        };
                    }
                    else {
                        fileSystemAPI = new LocalFileSystemAPI();
                    }

                    if (job.Type == BackupType.Full)
                        RunBackup(
                            new FullBackup(source.Directory, target.Directory, job.ID, fileSystemAPI)
                        );
                    else if (job.Type == BackupType.Differential)
                        RunBackup(
                            new DifferentialBackup(source.Directory, target.Directory, job.ID, fileSystemAPI, job.Retention)
                        );
                    else if (job.Type == BackupType.Incremental)
                        RunBackup(
                            new IncrementalBackup(source.Directory, target.Directory, job.ID, fileSystemAPI, job.Retention)
                        );
                }
            }
        }
        public static void RunBackup(Backup backup) {
            var progress = new Progress<BackupProgress>();
            progress.ProgressChanged += (s, e) => {
                LogItem report = new LogItem() {
                    JobID = backup.JobID,
                    Date = DateTime.Now,
                    Message = String.Format("{0} to {1} @ {2}%", backup.Source, backup.Target, e.Percentage),
                    Type = MessageType.Job
                };
                APIService.SendReport(report);
            };

            Task.Factory.StartNew(() => backup.Run(progress));
        }
    }
}
