using BackupServiceDaemon.Models;
using System;

namespace BackupServiceDaemon
{
    public class Application
    {
        public int ID { get; set; }
        Computer computer = new Computer();

        public bool IsRecorded()
        {
            foreach (var item in Http.GetComputer())
            {
                if (PCInfo.GetIP() == item.IP && PCInfo.GetMAC() == item.MAC)
                    return true;
            }
            return false;
        }
        public bool IsActive()
        {
            foreach (var item in Http.GetComputer())
            {
                if (PCInfo.GetIP() == item.IP
                   && PCInfo.GetMAC() == item.MAC)
                {
                    this.ID = Convert.ToInt32(item.ID);
                    if (item.Status == 1)
                        return true;
                    else
                        return false;
                }
            }            
            throw new Exception("Computer not recorded");
        }        
        public void Init(){
            if (!IsRecorded())
                Http.PostComputer();
            else
                System.Console.WriteLine("Computer already recorded");
            
            if(IsActive()){
                Console.WriteLine("Computer active");
            }
            else
                Console.WriteLine("Computer inactive");
        }
    }
}