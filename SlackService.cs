using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace MinecraftSlackIntegration
{
    [ServiceContract]
    public interface ISlackService
    {
        [WebInvoke(Method = "POST", UriTemplate = "PostSlackToMinecraft", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        void PostSlackToMinecraft(Stream request);
    }

    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class SlackService : ISlackService
    {
        public void PostSlackToMinecraft(Stream request)
        {
            StreamReader sr = new StreamReader(request);
            string raw = sr.ReadToEnd();
            sr.Close();

            SlackMessage msg = new SlackMessage(raw);
            SlackServiceHandler.HandlePostSlackToMinecraft(msg);
        }
    }

    public class SlackMessage
    {
        public string Token { get; set; }
        public string TeamID { get; set; }
        public string TeamDomain { get; set; }
        public string ChannelID { get; set; }
        public string ChannelName { get; set; }
        public string Timestamp { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public string TriggerWord { get; set; }

        public SlackMessage(string rawData)
        {
            DeserializeData(rawData);
        }

        private void DeserializeData(string rawData)
        {
            string propName = string.Empty, propData = string.Empty, temp = string.Empty;

            for (int i = 0; i < rawData.Length; i++)
            {
                if(rawData[i] == '=')
                {
                    propName = temp;
                    temp = string.Empty;
                }
                else if(rawData[i] == '&')
                {
                    propData = temp;
                    temp = string.Empty;
                    FillInVariable(propName, propData);
                }
                else
                {
                    temp += rawData[i];
                }
            }

            //Once more for the final property
            propData = temp;
            temp = string.Empty;
            FillInVariable(propName, propData);
        }

        private void FillInVariable(string propName, string propData)
        {
            switch (propName)
            {
                case "token":
                    this.Token = propData;
                    break;
                case "team_id":
                    this.TeamID = propData;
                    break;
                case "team_domain":
                    this.TeamDomain = propData;
                    break;
                case "channel_id":
                    this.ChannelID = propData;
                    break;
                case "channel_name":
                    this.ChannelName = propData;
                    break;
                case "timestamp":
                    this.Timestamp = propData;
                    break;
                case "user_id":
                    this.UserID = propData;
                    break;
                case "user_name":
                    this.UserName = propData;
                    break;
                case "text":
                    this.Text = propData.Replace('+', ' ');
                    break;
                case "trigger_word":
                    this.TriggerWord = propData;
                    break;
                default:
                    break;
            }
        }
    }
}
