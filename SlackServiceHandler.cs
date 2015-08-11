using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftSlackIntegration
{
    public class SlackServiceHandler
    {
        public static void HandlePostSlackToMinecraft(string token, string user_name, string text)
        {
            //Ignore empty text and bot messages
            if (string.IsNullOrEmpty(text) || user_name == "slackbot")
                return;

            //Check token validity
            if (token == ConfigurationManager.AppSettings["Slack_Token"].ToString())
            {
                MinecraftHandler.SayInGame(user_name, text);
            }
            else
                throw new UnauthorizedAccessException("Token values do not match.");
        }

        public static void SayInSlack(string username, string text)
        {
            //Create the request URL
            WebRequest request = WebRequest.Create(ConfigurationManager.AppSettings["Slack_URL"]);
            request.Method = "POST";
            request.ContentType = "text/json";

            //Serialize the json data
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string data_json = @"{""username"": """ + username + @""", ""text"": """ + text + @""", ""icon_url"": """ + String.Format("https://crafatar.com/avatars/{0}?date={1}", username, DateTime.Today.ToShortDateString()) + @"""}";
                streamWriter.Write(data_json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            //Get the response and log to console
            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine(result);
            }
        }
    }
}
