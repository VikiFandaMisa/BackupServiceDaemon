using System;
using System.Threading;

namespace BackupServiceDaemon
{
    class Program
    {
        Timer timer = new Timer(Tick, null, 0, 600000);
        static void Main(string[] args) {
            //Application.Login();
            Application.Loop();
        }
        public static void Tick(Object o)
		{			
			Application.Tick();
		}
    }
}
