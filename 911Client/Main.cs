using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.UI.Screen;
using static CitizenFX.Core.Native.API;

namespace _911
{
    public class Main : BaseScript
    {
        bool callActive = false;
        public Main()
        {
            EventHandlers.Add("911:AddBlipToMap", new Action<Vector3, string, int>(AddBlipToMap));

            RegisterCommand("911", new Action<int, List<dynamic>, string>((source, args, rawCommand) =>
            {
                if (!callActive)
                {
                    string name = Game.Player.Name;
                    string msg = string.Join(" ", args);
                    Vector3 coords = GetEntityCoords(Game.PlayerPed.Handle, true);
                    string location = World.GetStreetName(coords);
                    TriggerServerEvent("911:SendMessage", coords, name, location, msg);
                } else
                {
                    TriggerEvent("chatMessage", "You have a call currently active please wait.");
                }
            }), false);

            TriggerEvent("chat:addSuggestion", "/911", "Call for emergency services", new[]
            {
                new { name="Report", help="Enter your report here" }
            });
        }

        public async void AddBlipToMap(Vector3 coords, string name, int timeout) 
        {
            callActive = true;
            var blip = AddBlipForCoord(coords.X, coords.Y, coords.Z);
            SetBlipSprite(blip, 162);
            SetBlipColour(blip, 38);
            BeginTextCommandSetBlipName("STRING");
            AddTextComponentString("911 Report - " + name);
            EndTextCommandSetBlipName(blip);
            await Delay(timeout);
            RemoveBlip(ref blip);
            callActive = false;
        }
    }
}
