using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class DifferentialBackup : IBackup
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Differential backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
            if (IsFirst()) {
                FullBackup full = new FullBackup() { Source = this.Source, Target = this.Target };
                full.Backup();
            }
            else
                this.Backup();
        }
        public void Backup() {
            this.CopyChangedFiles();
			this.DeleteErasedFiles();
        }
        private bool IsFirst() {
            return true;
        }
        public void CopyChangedFiles() {
            var stack = new Stack<Folders>();
            stack.Push(new Folders(Source, Target));

            while (stack.Count > 0) {
                var folders = stack.Pop();
				foreach (var fileT in Directory.GetFiles(folders.Target, "*.*"))
                	foreach (var fileS in Directory.GetFiles(folders.Source, "*.*"))
						if ((File.GetLastAccessTimeUtc(fileS) != File.GetLastAccessTimeUtc(fileT)) && fileS == fileT) {
							File.Delete(Path.Combine(folders.Target, Path.GetFileName(fileT)));
							File.Copy(fileS, Path.Combine(folders.Target, Path.GetFileName(fileS)));							
						}

                foreach (var folder in Directory.GetDirectories(folders.Source))
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
            }
        }
		public void DeleteErasedFiles() {
			var stack = new Stack<Folders>();
            stack.Push(new Folders(Source, Target));
				
            while (stack.Count > 0) {
                var folders = stack.Pop();
				
				List<string> filesT = new List<string>(Directory.GetFiles(folders.Target, "*.*").ToList());
				List<string> filesS = new List<string>(Directory.GetFiles(folders.Source, "*.*").ToList());
				List<string> filesToDelete = filesT.Where(o => !filesS.Contains(o)).ToList();
				//var filesToDelete = filesT.Except(filesS);

				foreach (var fileToDelete in filesToDelete) {
					File.Delete(Path.Combine(folders.Target, Path.GetFileName(fileToDelete)));
				}

                foreach (var folder in Directory.GetDirectories(folders.Source))
                    stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
            }
		}
    }
}