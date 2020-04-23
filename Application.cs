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
            if (PCInfo.GetIP() == Http.GetComputer().IP && PCInfo.GetMAC() == Http.GetComputer().MAC)
                return true;
            return false;
        }
        public bool IsActive()
        {
            if (PCInfo.GetIP() ==  Http.GetComputer().IP && PCInfo.GetMAC() ==  Http.GetComputer().MAC) {
            this.ID = Convert.ToInt32( Http.GetComputer().ID);
                if ( Http.GetComputer().Status == 1)
                    return true;
                else
                    return false;
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