using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.NPCs.Friendly;
using Redemption.WorldGeneration;
using SubworldLibrary;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
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
        public static bool[] voltVars = new bool[4];
        public static int forestNymphVar;
        public static int calaviaVar;
        public override void PostUpdateWorld()
        {
            if (SubworldSystem.Current != null)
                return;

            #region Wayfarer Event
            if (wayfarerVars[0] == 0 && Main.dayTime && RedeWorld.DayNightCount >= 1 && !RedeHelper.WayfarerActive())
            {
                wayfarerVars[0] = 1;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);

                string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.PortalRumble");
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGreen);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightGreen);
            }
            if (wayfarerVars[0] == 2 && Main.dayTime && RedeWorld.DayNightCount >= 3 && !RedeHelper.WayfarerActive())
            {
                if (Main.time == 1)
                {
                    string w = Language.GetTextValue("Mods.Redemption.NPCs.Zephos_Intro.DisplayName");
                    if (WorldGen.crimson)
                        w = Language.GetTextValue("Mods.Redemption.NPCs.Daerel_Intro.DisplayName");
                    string status = w + Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.WayfarerReturn");
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(48, 121, 248));
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), new Color(48, 121, 248));

                    Vector2 anglonPortalPos = new(((RedeGen.newbCaveVector.X + 35) * 16) - 8, ((RedeGen.newbCaveVector.Y + 6) * 16) - 4);
                    int wayfarer = WorldGen.crimson ? ModContent.NPCType<Daerel>() : ModContent.NPCType<Zephos>();
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, anglonPortalPos);
                    for (int i = 0; i < 30; i++)
                    {
                        int dust = Dust.NewDust(anglonPortalPos, 24, 48, ModContent.DustType<GlowDust>(), 1, 0, 0, default, 0.5f);
                        Main.dust[dust].noGravity = true;
                        Color dustColor = new(Color.DarkOliveGreen.R, Color.DarkOliveGreen.G, Color.DarkOliveGreen.B) { A = 0 };
                        Main.dust[dust].color = dustColor;
                        Main.dust[dust].velocity *= 3f;
                    }
                    if (RedeGen.newbCaveVector.X != -1 && !RedeHelper.WayfarerActive())
                        LabArea.SpawnNPCInWorld(anglonPortalPos, wayfarer);

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
            }
            #endregion
            if (calaviaVar == 0 && Main.dayTime && Terraria.NPC.downedBoss3)
            {
                Point originPoint = RedeGen.gathicPortalVector.ToPoint();
                GenUtils.ObjectPlace(originPoint.X + 36, originPoint.Y + 17, TileID.Torches);

                calaviaVar = 1;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);

                string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.PortalRumble");
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightBlue);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightBlue);
            }

        }
        public override void ClearWorld()
        {
            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = 0;
            for (int k = 0; k < voltVars.Length; k++)
                voltVars[k] = false;
            forestNymphVar = 0;
            calaviaVar = 0;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            var lists = new List<string>();

            for (int k = 0; k < voltVars.Length; k++)
            {
                if (voltVars[k])
                    lists.Add("VV" + k);
            }
            tag["lists"] = lists;
            for (int k = 0; k < wayfarerVars.Length; k++)
                tag["WV" + k] = wayfarerVars[k];
            tag["FNV"] = forestNymphVar;
            tag["CV"] = calaviaVar;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var lists = tag.GetList<string>("lists");
            for (int k = 0; k < voltVars.Length; k++)
                voltVars[k] = lists.Contains("VV" + k);

            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = tag.GetInt("WV" + k);
            forestNymphVar = tag.GetInt("FNV");
            calaviaVar = tag.GetInt("CV");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            for (int k = 0; k < voltVars.Length; k++)
                flags[k] = voltVars[k];
            writer.Write(flags);

            for (int k = 0; k < wayfarerVars.Length; k++)
                writer.Write(wayfarerVars[k]);
            writer.Write(forestNymphVar);
            writer.Write(calaviaVar);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            for (int k = 0; k < voltVars.Length; k++)
                voltVars[k] = flags[k];

            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = reader.ReadInt32();
            forestNymphVar = reader.ReadInt32();
            calaviaVar = reader.ReadInt32();
        }
    }
}
