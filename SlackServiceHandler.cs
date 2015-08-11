using RestSharp;
using System;
using System.Configuration;
using System.Web;

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
            if (token == ConfigurationManager.AppSettings["Slack_Outgoing_Token"].ToString())
            {
                MinecraftHandler.SayInGame(user_name, text);
            }
            else
                throw new UnauthorizedAccessException("Token values do not match.");
        }

        public static void SayInSlack(string username, string text)
        {
            //Set the uri
            var uri = new Uri(ConfigurationManager.AppSettings["Slack_Incoming_URL"], UriKind.Absolute);

            //create a new rest request
            var request = new RestRequest(uri.PathAndQuery, Method.POST);

            //serialize the json data
            string data_json = @"{""username"": """ + username + @""", ""text"": """ + HttpUtility.HtmlEncode(text) + @""", ""icon_url"": """ + String.Format("https://crafatar.com/avatars/{0}?date={1}", username, DateTime.Today.ToShortDateString()) + @"""}";

            //Add paramter to the request
            request.AddParameter("payload", data_json);

            //Create a client
            var restClient = new RestClient(uri.GetLeftPart(UriPartial.Authority));

            //Send the request and receive a response
            var response = restClient.Execute(request);

            //Save response to console
            Console.WriteLine("Sending " + data_json + "\nResult: " + response.StatusDescription);
        }
    }
}
