using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class DifferentialBackup : IBackup {
		public string Source { get; set; }
		public string Target { get; set; }
		public string LastFullBU { get; set; }
		public void Run(IProgress<BackupProgress> progress) {
			System.Console.WriteLine("Differential backup");
			progress.Report(new BackupProgress() { Percentage = 100 });
			if (IsFirst()) {
				Utils.CopyDirectory(Source, Target);
			}
			else
				this.Backup();
		}
		public void Backup() {
			Utils.CopyChangedFiles(Source, Target, LastFullBU);
		}
		private bool IsFirst() {
			return true;
		}
	}
}