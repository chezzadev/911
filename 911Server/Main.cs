using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace _911Server
{
    public class Main : BaseScript
    {
        bool displayLocation = false;
        int timeout = 0;
        bool discord = false;
        string webhookurl = null;

        public Main()
        {
            LoadConfig();
            EventHandlers.Add("911:SendMessage", new Action<Player, Vector3, string, string, string>(SendMessage));
        }

        public void LoadConfig()
        {
            string config = null;
            try
            {
                config = LoadResourceFile(GetCurrentResourceName(), "config.ini");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error occurred while loading the config file.");
            }
            Config cfg = new Config(config);
            displayLocation = cfg.Get("location") == "true";
            string timeoutString = cfg.Get("blipTimeout", "30");
            if (int.TryParse(timeoutString, out int val))
            {
                timeout = val * 1000;
            }
            discord = cfg.Get("discord") == "true";
            if (discord)
            {
                webhookurl = cfg.Get("webhookurl").Replace('"', ' ').Trim();
            }
        }

        public void SendMessage([FromSource] Player source, Vector3 coords, string name, string location, string msg)
        {
            if (displayLocation)
            {
                TriggerClientEvent("chatMessage", "^*^5911 ^r^7- " + name + " ^*^5[^r" + location + "^*^5]^r: " + msg);
                TriggerClientEvent("911:AddBlipToMap", coords, name, timeout);
                if (discord)
                {
                    TriggerEvent("911:SendDiscordMessage", name, msg, location, webhookurl);
                }
            } else
            {
                TriggerClientEvent("chatMessage", "^*^5911 ^r^7- " + name + ": " + msg);
                if (discord)
                {
                    TriggerEvent("911:SendDiscordMessage", name, msg, "Disabled", webhookurl);
                }
            }
        }
    }
}
