using Microsoft.Xna.Framework;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Friendly;
using System;
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
        public static int tbotDownedTimer;
        public static int daerelDownedTimer;
        public static int zephosDownedTimer;
        public static bool spawnWayfarer;
        public static float RotTime;

        public override void PreUpdateWorld()
        {
            RotTime += (float)Math.PI / 60;
            if (RotTime >= Math.PI * 2) RotTime = 0;
        }

        public override void PostUpdateNPCs()
        {
            if (Terraria.NPC.AnyNPCs(ModContent.NPCType<TBotUnconscious>()))
                tbotDownedTimer++;
            if (Terraria.NPC.AnyNPCs(ModContent.NPCType<DaerelUnconscious>()))
                daerelDownedTimer++;
            if (Terraria.NPC.AnyNPCs(ModContent.NPCType<ZephosUnconscious>()))
                zephosDownedTimer++;
        }

        public override void PostUpdateWorld()
        {
            if (Main.time == 1)
                DayNightCount++;

            #region Wayfarer Event
            if (DayNightCount >= 2 && Main.time == 1 && !RedeHelper.WayfarerActive())
            {
                spawnWayfarer = true;

                string status = "Someone travelled through the surface portal...";
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGreen);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightGreen);
            }
            #endregion

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

            int PalebatImpID = Terraria.NPC.FindFirstNPC(ModContent.NPCType<PalebatImp>());
            if (PalebatImpID >= 0 && (Main.npc[PalebatImpID].ModNPC as PalebatImp).shakeTimer > 0)
            {
                if (!Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene.Activate("MoonLordShake", Main.player[Main.myPlayer].position);
                }
                Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].GetShader().UseIntensity((Main.npc[PalebatImpID].ModNPC as PalebatImp).shakeTimer);
            }

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
            tbotDownedTimer = 0;
            daerelDownedTimer = 0;
            zephosDownedTimer = 0;
            spawnWayfarer = false;
        }

        public override void OnWorldUnload()
        {
            alignment = 0;
            DayNightCount = 0;
            SkeletonInvasion = false;
            spawnKeeper = false;
            spawnSkeletonInvasion = false;
            tbotDownedTimer = 0;
            daerelDownedTimer = 0;
            zephosDownedTimer = 0;
            spawnWayfarer = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var lists = new List<string>();

            if (SkeletonInvasion)
                lists.Add("SkeletonInvasion");

            tag["lists"] = lists;
            tag["alignment"] = alignment;
            tag["DayNightCount"] = DayNightCount;
            tag["tbotDownedTimer"] = tbotDownedTimer;
            tag["daerelDownedTimer"] = daerelDownedTimer;
            tag["zephosDownedTimer"] = zephosDownedTimer;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var lists = tag.GetList<string>("lists");

            SkeletonInvasion = lists.Contains("SkeletonInvasion");
            alignment = tag.GetInt("alignment");
            DayNightCount = tag.GetInt("DayNightCount");
            tbotDownedTimer = tag.GetInt("tbotDownedTimer");
            daerelDownedTimer = tag.GetInt("daerelDownedTimer");
            zephosDownedTimer = tag.GetInt("zephosDownedTimer");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = SkeletonInvasion;
            writer.Write(flags);

            writer.Write(alignment);
            writer.Write(DayNightCount);
            writer.Write(tbotDownedTimer);
            writer.Write(daerelDownedTimer);
            writer.Write(zephosDownedTimer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            SkeletonInvasion = flags[0];

            alignment = reader.ReadInt32();
            DayNightCount = reader.ReadInt32();
            tbotDownedTimer = reader.ReadInt32();
            daerelDownedTimer = reader.ReadInt32();
            zephosDownedTimer = reader.ReadInt32();
        }
    }
}