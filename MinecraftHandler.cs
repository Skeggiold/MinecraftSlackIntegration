using MinecraftServerRCON;
using System;
using System.Configuration;
using System.Web;

namespace MinecraftSlackIntegration
{
    class MinecraftHandler
    {
        public static void SayInGame(string username, string text)
        {
            //Create the JSON data
            string json_data = @"{ ""text"" : ""< " + username + @"> " + HttpUtility.HtmlDecode(text) + @""" }";
            using (var rcon = RCONClient.INSTANCE)
            {
                //Send the json data via the /tellraw command, through RCON, to the minecraft server
                //View http://minecraft.gamepedia.com/Commands#tellraw for more info
                rcon.setupStream(ConfigurationManager.AppSettings["Rcon_IP"], Convert.ToInt32(ConfigurationManager.AppSettings["Rcon_Port"]), ConfigurationManager.AppSettings["Rcon_Password"]);
                string command = @"/tellraw @a " + json_data;
                Console.WriteLine(command);
                var answer = rcon.sendMessage(RCONMessageType.Command, command);
                Console.WriteLine(answer.RemoveColorCodes());
            }
        }
    }
}
