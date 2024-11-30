using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.NPCs.Friendly.TownNPCs;
using Redemption.Tiles.Natural;
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
using static Redemption.Globals.RedeNet;

namespace Redemption.Globals
{
    public class RedeQuest : ModSystem
    {
        public static int[] wayfarerVars = new int[2];
        public static bool[] voltVars = new bool[4];
        public static int forestNymphVar;
        public static int calaviaVar;
        public static int slayerRep;

        public static bool[] adviceUnlocked = new bool[6];
        public static bool[] adviceSeen = new bool[20];

        // Second row is for the advice not needing unlocking, so adviceUnlocked and adviceSeen can use same enum numbers even though its different sized arrays
        // Add new unlockable advice before "Elements"
        public enum Advice : byte
        {
            UGPortal, ForestNymph, UkkoEye, EaglecrestGolem, Androids, StarSerpent,
            Elements, Insects, Invisibility, GuardPoints, DirtyWound, Fool, Chalice, Spirits, Undead, Slimes, Erhan
        }

        public override void PostUpdateWorld()
        {
            if (SubworldSystem.Current != null)
                return;

            WayfarerEvent();

            if (calaviaVar == 0 && Main.dayTime && Terraria.NPC.downedBoss3)
            {
                Point originPoint = RedeGen.gathicPortalVector.ToPoint();
                GenUtils.ObjectPlace(originPoint.X + 36, originPoint.Y + 17, TileID.Torches);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    calaviaVar = 1;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }

                string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.PortalRumble");
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightBlue);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightBlue);
            }
        }

        static void WayfarerEvent()
        {
            if (RedeGen.newbCaveVector.X == -1)
                return;
            if (wayfarerVars[0] == 0 && Main.dayTime && RedeWorld.DayNightCount >= 1 && !RedeHelper.WayfarerActive())
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    wayfarerVars[0] = 1;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
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
                    string w = Lang.GetNPCNameValue(NPCType<Zephos_Intro>());
                    if (WorldGen.crimson)
                        w = Lang.GetNPCNameValue(NPCType<Daerel_Intro>());
                    string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.Return", w);
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(50, 125, 255));
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), new Color(50, 125, 255));

                    Vector2 anglonPortalPos = new(((RedeGen.newbCaveVector.X + 35) * 16) - 8, ((RedeGen.newbCaveVector.Y + 6) * 16) - 4);
                    int wayfarer = WorldGen.crimson ? ModContent.NPCType<Daerel>() : ModContent.NPCType<Zephos>();
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, anglonPortalPos);
                    for (int i = 0; i < 30; i++)
                    {
                        int dust = Dust.NewDust(anglonPortalPos - new Vector2(12, 24), 24, 48, ModContent.DustType<GlowDust>(), 1, 0, 0, default, 0.5f);
                        Main.dust[dust].noGravity = true;
                        Color dustColor = new(Color.DarkOliveGreen.R, Color.DarkOliveGreen.G, Color.DarkOliveGreen.B) { A = 0 };
                        Main.dust[dust].color = dustColor;
                        Main.dust[dust].velocity *= 3f;
                    }
                    if (!RedeHelper.WayfarerActive())
                        LabArea.SpawnNPCInWorld(anglonPortalPos, wayfarer);
                }
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
            slayerRep = 0;

            for (int k = 0; k < adviceUnlocked.Length; k++)
                adviceUnlocked[k] = false;
            for (int k = 0; k < adviceSeen.Length; k++)
                adviceSeen[k] = false;
        }
        public override void SaveWorldData(TagCompound tag)
        {
            var lists = new List<string>();

            for (int k = 0; k < voltVars.Length; k++)
            {
                if (voltVars[k])
                    lists.Add("VV" + k);
            }
            for (int k = 0; k < adviceUnlocked.Length; k++)
            {
                if (adviceUnlocked[k])
                    lists.Add("AdviceU" + k);
            }
            for (int k = 0; k < adviceSeen.Length; k++)
            {
                if (adviceSeen[k])
                    lists.Add("AdviceS" + k);
            }
            tag["lists"] = lists;

            for (int k = 0; k < wayfarerVars.Length; k++)
                tag["WV" + k] = wayfarerVars[k];
            tag["FNV"] = forestNymphVar;
            tag["CV"] = calaviaVar; 
            tag["slayerRep"] = slayerRep;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var lists = tag.GetList<string>("lists");
            for (int k = 0; k < voltVars.Length; k++)
                voltVars[k] = lists.Contains("VV" + k);

            for (int k = 0; k < adviceUnlocked.Length; k++)
                adviceUnlocked[k] = lists.Contains("AdviceU" + k);
            for (int k = 0; k < adviceSeen.Length; k++)
                adviceSeen[k] = lists.Contains("AdviceS" + k);

            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = tag.GetInt("WV" + k);
            forestNymphVar = tag.GetInt("FNV");
            calaviaVar = tag.GetInt("CV");
            slayerRep = tag.GetInt("slayerRep");
        }

        #region Netcode
        /// <summary>
        /// Syncs RedeQuest data. Can be called on clients or server, you must make sure only one of them calls it.
        /// </summary>
        public static void SyncData()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Redemption.Instance.GetPacket();
                packet.Write((byte)ModMessageType.SyncRedeQuestFromClient);
                ModContent.GetInstance<RedeQuest>().NetSend(packet);
                packet.Send();
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }

        public static void ReceiveSyncDataFromClient(BinaryReader reader, int sender)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            ModContent.GetInstance<RedeQuest>().NetReceive(reader);
            NetMessage.SendData(MessageID.WorldData, ignoreClient: sender);
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            for (int k = 0; k < voltVars.Length; k++)
                flags[k] = voltVars[k];
            writer.Write(flags);

            BitsByte[] adviceUnlockFlags = new BitsByte[3];
            int flagID = 0;
            for (int k = 0; k < adviceUnlocked.Length; k++)
            {
                if (k % 8 == 0)
                    flagID++;

                adviceUnlockFlags[flagID][k] = adviceUnlocked[k];
            }
            for (int k = 0; k < adviceUnlockFlags.Length; k++)
                writer.Write(adviceUnlockFlags[k]);

            BitsByte[] adviceSeenFlags = new BitsByte[3];
            flagID = 0;
            for (int k = 0; k < adviceSeen.Length; k++)
            {
                if (k % 8 == 0)
                    flagID++;

                adviceSeenFlags[flagID][k] = adviceSeen[k];
            }
            for (int k = 0; k < adviceSeenFlags.Length; k++)
                writer.Write(adviceSeenFlags[k]);

            for (int k = 0; k < wayfarerVars.Length; k++)
                writer.Write(wayfarerVars[k]);
            writer.Write(forestNymphVar);
            writer.Write(calaviaVar);
            writer.Write(slayerRep);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            for (int k = 0; k < voltVars.Length; k++)
                voltVars[k] = flags[k];

            BitsByte[] adviceUnlockFlags = new BitsByte[3] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            int flagID = 0;
            for (int k = 0; k < adviceUnlocked.Length; k++)
            {
                if (k % 8 == 0)
                    flagID++;

                adviceUnlocked[k] = adviceUnlockFlags[flagID][k];
            }
            BitsByte[] adviceSeenFlags = new BitsByte[3] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            flagID = 0;
            for (int k = 0; k < adviceSeen.Length; k++)
            {
                if (k % 8 == 0)
                    flagID++;

                adviceSeen[k] = adviceSeenFlags[flagID][k];
            }

            for (int k = 0; k < wayfarerVars.Length; k++)
                wayfarerVars[k] = reader.ReadInt32();
            forestNymphVar = reader.ReadInt32();
            calaviaVar = reader.ReadInt32();
            slayerRep = reader.ReadInt32();
        }
        #endregion
    }
}