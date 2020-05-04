using System;
using System.Collections.Generic;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public static class Utils {
		public static void CopyDirectory(string source, string target) {
			var stack = new Stack<TwoFolders>();
			stack.Push(new TwoFolders(source, target));

			while (stack.Count > 0) {
				var folders = stack.Pop();
				Directory.CreateDirectory(folders.Target);
				foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
					File.Copy(file, Path.Combine(folders.Target, Path.GetFileName(file)));

				foreach (var folder in Directory.GetDirectories(folders.Source))
					stack.Push(new TwoFolders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
			}
		}
		public static void CopyChangedFiles(string source, string target, string lastBU) {
			var stack = new Stack<ThreeFolders>();
			stack.Push(new ThreeFolders(source, target, lastBU));

			while (stack.Count > 0) {
				var folders = stack.Pop();
				foreach (var fileS in Directory.GetFiles(folders.Source, "*.*")) {
					foreach (var fileL in Directory.GetFiles(folders.LastBU, "*.*")) {
						if ((File.GetLastWriteTimeUtc(fileS) != File.GetLastWriteTimeUtc(fileL))) {
							Directory.CreateDirectory(folders.Target);
							File.Copy(fileS, Path.Combine(folders.Target, Path.GetFileName(fileS)));
						}
					}
				}

				foreach (var folder in Directory.GetDirectories(folders.Source))
					stack.Push(new ThreeFolders(folder, Path.Combine(folders.Target, Path.GetFileName(folder)), Path.Combine(folders.LastBU, Path.GetFileName(folder))));
			}
		}
	}
	public class TwoFolders {
		public string Source { get; private set; }
		public string Target { get; private set; }

		public TwoFolders(string source, string target) {
			Source = source;
			Target = target;
		}
	}
	public class ThreeFolders {
		public string Source { get; private set; }
		public string Target { get; private set; }
		public string LastBU { get; set; }

		public ThreeFolders(string source, string target, string lastBU) {
			Source = source;
			Target = target;
			LastBU = lastBU;
		}
	}
}