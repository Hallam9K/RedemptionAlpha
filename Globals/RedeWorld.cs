using Microsoft.Xna.Framework;
using Redemption.NPCs.Bosses.Keeper;
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
        public static bool spawnSkeletonInvasion;
        public static bool spawnKeeper;

        public override void PostUpdateWorld()
        {
            if (Main.time == 1)
                DayNightCount++;

            #region Skeleton Invasion
            if (DayNightCount >= 10 && !Main.hardMode && !Main.fastForwardTime)
            {
                if (Main.dayTime && Main.time == 1 && !WorldGen.spawnEye)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!RedeBossDowned.downedSkeletonInvasion || (DayNightCount > 12 && DayNightCount % 3 == 0 && Main.rand.NextBool(8)))
                        {
                            spawnSkeletonInvasion = true;

                            string status = "The skeletons are plotting an invasion at dusk..." + (RedeBossDowned.downedSkeletonInvasion ? " Again." : "");
                            if (Main.netMode == NetmodeID.Server)
                                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGray);
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue(status), Color.LightGray);
                        }
                    }
                }
                if (!Main.dayTime && spawnSkeletonInvasion && Main.netMode != NetmodeID.MultiplayerClient && Main.time > 1)
                {
                    string status = "The skeletons are invading!";
                    if (WorldGen.spawnEye || Main.bloodMoon)
                        status = "The skeletons reconsidered invading tonight...";
                    else
                        SkeletonInvasion = true;

                    spawnSkeletonInvasion = false;

                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGray);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.LightGray);

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (SkeletonInvasion && (Main.time >= 16200 || Main.dayTime))
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

            #region Keeper Summoning
            if (!Main.dayTime && Terraria.NPC.downedBoss1 && !Main.hardMode && !Main.fastForwardTime)
            {
                if (Main.time == 1 && !WorldGen.spawnEye && !spawnSkeletonInvasion)
                {
                    if (!RedeBossDowned.downedKeeper && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        bool check = false;
                        for (int n = 0; n < Main.maxPlayers; n++)
                        {
                            Terraria.Player player = Main.player[n];
                            if (!player.active || player.statLifeMax < 200 || player.statDefense <= 10)
                                continue;

                            check = true;
                            break;
                        }
                        if (check && Main.rand.NextBool(3))
                        {
                            spawnKeeper = true;

                            string status = "Shrieks echo through the night...";
                            if (Main.netMode == NetmodeID.Server)
                                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.MediumPurple);
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue(status), Color.MediumPurple);
                        }
                    }
                }
                if (spawnKeeper && Main.netMode != NetmodeID.MultiplayerClient && Main.time > 4860)
                {
                    for (int k = 0; k < Main.maxPlayers; k++)
                    {
                        Terraria.Player player = Main.player[k];
                        if (!player.active || player.dead || player.position.Y >= Main.worldSurface * 16.0)
                            continue;

                        int type = ModContent.NPCType<Keeper>();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Terraria.NPC.SpawnOnPlayer(player.whoAmI, type);
                        else
                            NetMessage.SendData(MessageID.SpawnBoss, number: player.whoAmI, number2: type);

                        spawnKeeper = false;
                        break;
                    }
                }
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
            spawnKeeper = false;
            spawnSkeletonInvasion = false;
        }

        public override void OnWorldUnload()
        {
            alignment = 0;
            DayNightCount = 0;
            SkeletonInvasion = false;
            spawnKeeper = false;
            spawnSkeletonInvasion = false;
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