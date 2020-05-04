using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class IncrementalBackup : IBackup {
		public string Source { get; set; }
		public string Target { get; set; }
		public string LastBU { get; set; }
        public int Retention { get; set; } = 2;
		public void Run(IProgress<BackupProgress> progress) {
			System.Console.WriteLine("Incremental backup");
			progress.Report(new BackupProgress() { Percentage = 100 });
		}
		public void Backup() {
            Directory.CreateDirectory(Target + @"\BackUp_" + DateTime.Today.ToShortDateString());
			if (Utils.IsLimitReached(Target, Retention) || Utils.IsFirst(Target)) {
				Directory.CreateDirectory(Target + @"\BackUp_" + DateTime.Today.ToShortDateString());
				Utils.CopyDirectory(Source, Target + @"\BackUp_" + DateTime.Today.ToShortDateString());
            }
			else {
				Directory.CreateDirectory(Target + @"\BackUp_" + DateTime.Today.ToShortDateString());
				Utils.CopyChangedFiles(Source, Target + @"\BackUp_" + DateTime.Today.ToShortDateString(), LastBU);
            }
		}
	}
}