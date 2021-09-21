using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
    public class RedeWorld : ModSystem
    {
        public static bool blobbleSwarm;
        public static int blobbleSwarmTimer;
        public static int blobbleSwarmCooldown;
        public static int alignment;
        public static int DayNightCount;
        public static bool SkeletonInvasion;

        public override void PostUpdateWorld()
        {
            if (Main.time == 1)
                DayNightCount++;

            #region Skeleton Invasion
            if (DayNightCount == 10 && Main.time == 1)
            {
                string status = "The skeletons are plotting an invasion at dusk...";
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGray);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightGray);
            }
            if (DayNightCount == 11 && Main.time == 1)
            {
                WorldGen.spawnEye = false;
                SkeletonInvasion = true;
                string status = "The skeletons are invading!";
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGray);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightGray);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            if (!RedeBossDowned.downedSkeletonInvasion && DayNightCount >= 11 && Main.time >= 16200)
            {
                SkeletonInvasion = false;
                RedeBossDowned.downedSkeletonInvasion = true;

                string status = "The skeletons got bored and went home!";
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGray);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightGray);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            #endregion

            if (blobbleSwarm)
            {
                blobbleSwarmTimer++;
                if (blobbleSwarmTimer > 180)
                {
                    blobbleSwarm = false;
                    blobbleSwarmTimer = 0;
                    blobbleSwarmCooldown = 86400;
                }
            }
            if (blobbleSwarmCooldown > 0)
                blobbleSwarmCooldown--;
        }

        public override void OnWorldLoad()
        {
            alignment = 0;
            DayNightCount = 0;
            SkeletonInvasion = false;
        }

        public override void OnWorldUnload()
        {
            alignment = 0;
            DayNightCount = 0;
            SkeletonInvasion = false;
        }

        public override TagCompound SaveWorldData()
        {
            var lists = new List<string>();

            if (SkeletonInvasion)
                lists.Add("SkeletonInvasion");

            return new TagCompound
            {
                ["lists"] = lists,
                ["alignment"] = alignment,
                ["DayNightCount"] = DayNightCount
            };
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var lists = tag.GetList<string>("lists");

            SkeletonInvasion = lists.Contains("SkeletonInvasion");
            alignment = tag.GetInt("alignment");
            DayNightCount = tag.GetInt("DayNightCount");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = SkeletonInvasion;
            writer.Write(flags);

            writer.Write(alignment);
            writer.Write(DayNightCount);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            SkeletonInvasion = flags[0];

            alignment = reader.ReadInt32();
            DayNightCount = reader.ReadInt32();
        }
    }
}