using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;


namespace GEMServiceController
{
    class Program
    {
        public enum SimpleServiceCustomCommands
        { StopWorker = 128, RestartWorker, CheckWorker };
        static void Main(string[] args)
        {
            const string gem = "GEMservice";

            ServiceController[] scServices = ServiceController.GetServices();



            foreach (ServiceController scTemp in scServices)
            {
                if (scTemp.ServiceName != gem) continue;
                ServiceController sc = new ServiceController(gem);

                string gemstatus = $"[ {DateTime.Now.ToString()} ] -- GEMservice Status: {sc.Status}";

                Console.WriteLine(gemstatus);


                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                    while (sc.Status == ServiceControllerStatus.Stopped)
                    {
                        Thread.Sleep(1000);
                        sc.Refresh();
                    }
                }

                // Issue custom commands to the service
                // enum ServiceCustomCommands
                //    { StopWorker = 128, RestartWorker, CheckWorker };
                sc.ExecuteCommand((int)GetStopWorker());
                sc.ExecuteCommand((int)SimpleServiceCustomCommands.RestartWorker);
                sc.Pause();


                while (sc.Status != ServiceControllerStatus.Paused)
                {
                    Thread.Sleep(1000);
                    sc.Refresh();
                }
                Console.WriteLine(gemstatus + sc.Status);
                sc.Continue();

                while (sc.Status == ServiceControllerStatus.Paused)
                {
                    Thread.Sleep(1000);
                    sc.Refresh();
                }
                Console.WriteLine(gemstatus + sc.Status);
                sc.Stop();

                while (sc.Status != ServiceControllerStatus.Stopped)
                {
                    Thread.Sleep(1000);
                    sc.Refresh();
                }
                Console.WriteLine(gemstatus + sc.Status);

                string[] argArray = new string[] { "ServiceController arg1", "ServiceController arg2" };
                sc.Start(argArray);

                while (sc.Status == ServiceControllerStatus.Stopped)
                {
                    Thread.Sleep(1000);
                    sc.Refresh();
                }

                Console.WriteLine(gemstatus + sc.Status);


                // Display the event log entries for the custom commands
                // and the start arguments.
                EventLog events = new EventLog("Application");
                EventLogEntryCollection entries = events.Entries;

                foreach (EventLogEntry entry in entries)
                {
                    if (!(entry.Source.IndexOf("SimpleService.OnCustomCommand", StringComparison.Ordinal) >= 0 |
                          entry.Source.IndexOf("SimpleService.Arguments", StringComparison.Ordinal) >= 0)) continue;
                    Console.WriteLine(entry.Message);
                }
            }
        }

        private static SimpleServiceCustomCommands GetStopWorker()
        {
            return SimpleServiceCustomCommands.StopWorker;
        }
    }
}