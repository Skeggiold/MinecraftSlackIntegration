using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MinecraftSlackIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.Invoke(() => HostWebservice(), () => MonitorMinecraftLog());
        }

        private static object MonitorMinecraftLog()
        {
            using (FileStream fs = new FileStream(ConfigurationManager.AppSettings["Minecraft_log"], FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while (true)
                    {
                        while (!sr.EndOfStream)
                            ProcessLine(sr.ReadLine());
                        while (sr.EndOfStream)
                            Thread.Sleep(100);
                        ProcessLine(sr.ReadLine());
                    }
                }
            }
        }

        private static void ProcessLine(string logLine)
        {
            //check whether the line in the logging is actually a chat message
            Regex regex = new Regex(@"^(.*?)INFO\]\: \<(.*?)\> (.*?)$");
            if (regex.IsMatch(logLine))
            {
                string[] splitLogLine = regex.Split(logLine);
                SlackServiceHandler.SayInSlack(splitLogLine[2], splitLogLine[3]);
            }
        }

        private static object HostWebservice()
        {
            //Permissions need to be given through the following command:
            //C:\Windows\system32>netsh http add urlacl url=http://+:80/hook user=DOMAIN\user
            WebHttpBinding binding = new WebHttpBinding();
            WebServiceHost host = new WebServiceHost(typeof(SlackService));
            host.AddServiceEndpoint(typeof(ISlackService), binding, "http://localhost:25555/SlackService");
            host.Open();
            while (true)
            {

            }
        }
    }
}
