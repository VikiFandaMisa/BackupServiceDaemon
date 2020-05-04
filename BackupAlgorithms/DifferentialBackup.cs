using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
	public class DifferentialBackup : IBackup {
		public string Source { get; set; }
		public string Target { get; set; }
		public string LastFullBU { get; set; }
		public int Retention { get; set; } = 2;
		public void Run(IProgress<BackupProgress> progress) {
			System.Console.WriteLine("Differential backup");
			progress.Report(new BackupProgress() { Percentage = 100 });
			if (Utils.IsFirst(Target) || Utils.IsLimitReached(Target, Retention)) {
				Directory.CreateDirectory(Target + @"\BackUp_" + DateTime.Today.ToShortDateString());
				Utils.CopyDirectory(Source, Target + @"\BackUp_" + DateTime.Today.ToShortDateString());
			}
			else {
				Directory.CreateDirectory(Target + @"\BackUp_" + DateTime.Today.ToShortDateString());
				this.Backup();
			}
		}
		public void Backup() {
			Utils.CopyChangedFiles(Source, Target + @"\BackUp_" + DateTime.Today.ToShortDateString(), LastFullBU);
		}
	}
}