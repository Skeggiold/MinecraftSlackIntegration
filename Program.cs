using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
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
                SlackServiceHandler.SayInSlack(regex.GetGroupNames()[1], regex.GetGroupNames()[2]);
            }
        }

        private static object HostWebservice()
        {
            WebServiceHost host = new WebServiceHost(typeof(SlackService));
            host.Open();
            while (true)
            {

            }
        }
    }
}
