using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
    public class RedeQuest : ModSystem
    {
        public static int[] wayfarerVars = new int[2];
        public override void PostUpdateWorld()
        {
            #region Wayfarer Event
            if (wayfarerVars[0] == 0 && Main.time == 1 && RedeWorld.DayNightCount >= 1 && !RedeHelper.WayfarerActive())
            {
                wayfarerVars[0] = 1;

                string status = "A portal rumbles... (Check Minimap for the location)";
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGreen);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightGreen);
            }
            #endregion
        }
        public override void OnWorldLoad()
        {
            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = 0;
        }
        public override void OnWorldUnload()
        {
            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = 0;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            for (int k = 0; k < wayfarerVars.Length; k++)
                tag["WV" + k] = wayfarerVars[k];
        }

        public override void LoadWorldData(TagCompound tag)
        {
            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = tag.GetInt("WV" + k);
        }

        public override void NetSend(BinaryWriter writer)
        {
            for (int k = 0; k < wayfarerVars.Length; k++)
                writer.Write(wayfarerVars[k]);
        }

        public override void NetReceive(BinaryReader reader)
        {
            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = reader.ReadInt32();
        }
    }
}