using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftSlackIntegration
{
    [ServiceContract]
    public interface ISlackService
    {
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, UriTemplate = "hook?format=json", BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract]
        void PostSlackToMinecraft(string token, string team_id, string team_domain, string channel_id, string channel_name, string timestamp, string user_id, string user_name, string text, string trigger_word);
    }
    public class SlackService : ISlackService
    {
        public void PostSlackToMinecraft(string token, string team_id, string team_domain, string channel_id, string channel_name, string timestamp, string user_id, string user_name, string text, string trigger_word)
        {
            SlackServiceHandler.HandlePostSlackToMinecraft(token, user_name, text);
        }
    }
}
