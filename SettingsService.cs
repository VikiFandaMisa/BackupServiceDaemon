using System;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using static System.Environment;

namespace BackupServiceDaemon
{
    public static class SettingsService
    {
        public static string ApplicationData { get; set; }
        public static string SettingsFile { get { return Path.Combine(ApplicationData, "settings.json"); } }
        public static Settings Settings { get; set; }
        static SettingsService() {
            ApplicationData = Path.Combine(
                GetFolderPath(SpecialFolder.ApplicationData),
                Path.GetFileNameWithoutExtension(
                    System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name
                )
            );

            if (!Directory.Exists(ApplicationData))
                Directory.CreateDirectory(ApplicationData);

            Settings = new Settings();
        }
        public static async void Save() {
            using (FileStream fs = File.Create(SettingsFile))
            {
                await JsonSerializer.SerializeAsync(fs, Settings);
            }
        }
        public static void Load() {
            Settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsFile));
        }
        public static void Wipe() {
            Directory.Delete(ApplicationData, true);
            Application.Exit = true;
        }
    }

    public class Settings
    {
        public int? ID { get; set; }
        public string Server { get; set; }
        public string Prefix { get; set; } = "Backup_";
        public string PrefixFull { get; set; } = "FullBackup_";
        public BackupServiceDaemon.Models.Job[] Jobs {get; set;}
    }
}