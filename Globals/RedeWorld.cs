using Microsoft.Xna.Framework;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Friendly;
using Redemption.Projectiles.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

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
        public static int slayerRep;

        #region Nuke Shenanigans
        public static int nukeTimerInternal = 1800;
        public static int nukeTimerShown = 30;
        public static int nukeFireballRadius = 287;
        public static bool nukeCountdownActive = false;
        public static Vector2 nukeGroundZero = Vector2.Zero;
        #endregion

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

                            string status = "The skeletons are plotting a party at dusk..." + (RedeBossDowned.downedSkeletonInvasion ? " Again." : "");
                            if (Main.netMode == NetmodeID.Server)
                                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGray);
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue(status), Color.LightGray);
                        }
                    }
                }
                if (!Main.dayTime && spawnSkeletonInvasion && Main.netMode != NetmodeID.MultiplayerClient && Main.time > 1)
                {
                    string status = "The skeletons are partying!";
                    if (WorldGen.spawnEye || Main.bloodMoon || WorldGen.spawnHardBoss > 0)
                        status = "The skeletons reconsidered partying tonight...";
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

            UpdateNukeCountdown();
        }

        #region Warhead Countdown
        public void UpdateNukeCountdown()
        {
            if (!nukeCountdownActive)
            {
                nukeTimerInternal = 1800;
                return;
            }
            else if (nukeGroundZero == Vector2.Zero)
            {
                nukeCountdownActive = false;
                return;
            }
            else
            {
                nukeTimerShown = nukeTimerInternal / 60;
                if (nukeTimerInternal % 60 == 0 && nukeTimerInternal > 0)
                {
                    if (RedeConfigClient.Instance.NoLoreElements)
                    {
                        Main.NewText(nukeTimerShown.ToString(), Color.Red);
                    }
                    else
                    {
                        if (!Main.dedServ)
                        {
                            RedeSystem.Instance.DialogueUIElement.DisplayDialogue(nukeTimerShown.ToString(), 40, 8, 1, null, ((30f - nukeTimerShown) / 30f) * 2, Color.Red, Color.Black);
                        }
                    }
                }
                --nukeTimerInternal;
                if (nukeTimerInternal <= 0)
                {
                    MoonlordDeathDrama.RequestLight(1f, nukeGroundZero);
                    for (int i = 0; i < Main.maxPlayers; ++i)
                    {
                        Terraria.Player player = Main.player[i];
                        if (!player.active || player.dead)
                            continue;

                        if (Vector2.Distance(player.Center, nukeGroundZero) < 287 * 16)
                            MoonlordDeathDrama.RequestLight(1f, player.Center);
                        else if (Vector2.Distance(player.Center, nukeGroundZero) < 287 * 2 * 16)
                            MoonlordDeathDrama.RequestLight(0.5f, player.Center);
                        else
                            MoonlordDeathDrama.RequestLight(0.35f, player.Center);
                    }
                }
                if (nukeTimerInternal <= -60)
                {
                    RedeHelper.ProjectileExplosion(new ProjectileSource_TileBreak((int)nukeGroundZero.X, (int)nukeGroundZero.Y), nukeGroundZero, 0, 90, ModContent.ProjectileType<NukeShockwave>(), 1, 80, nukeGroundZero.X, nukeGroundZero.Y);
                    HandleNukeExplosion();
                    WorldGen.KillTile((int)(nukeGroundZero.X / 16), (int)(nukeGroundZero.Y / 16), false, false, true);
                    ConversionHandler.ConvertWasteland(nukeGroundZero, 287);
                    nukeCountdownActive = false;
                    nukeGroundZero = Vector2.Zero;
                    RedeBossDowned.nukeDropped = true;
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
            }
        }

        public void HandleNukeExplosion()
        {
            for (int i = 0; i < Main.maxPlayers; ++i)
            {
                Terraria.Player player = Main.player[i];
                if (!player.active || player.dead)
                    continue;

                if (player.Distance(nukeGroundZero) < 287 * 16)
                {
                    string nukeDeathReason;

                    WeightedRandom<string> nukeDeaths = new(Main.rand);
                    nukeDeaths.Add(player.name + " saw a second sunrise.", 5);
                    nukeDeaths.Add(player.name + " was wiped off the face of " + Main.worldName + ".", 5);
                    nukeDeaths.Add(player.name + " experienced doomsday.", 5);
                    nukeDeaths.Add(player.name + " became a shadow on the ground.", 5);
                    nukeDeaths.Add(player.name + " went out with a bang.", 5);
                    nukeDeaths.Add(player.name + " couldn't find the fridge in time.", 1);

                    nukeDeathReason = nukeDeaths;
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/NukeExplosion"), player.position);

                    player.KillMe(PlayerDeathReason.ByCustomReason(nukeDeathReason), 999999, 1);
                }
                if (player.Distance(nukeGroundZero) < 287 * 2 * 16 && Collision.CanHit(player.position, player.width, player.height, nukeGroundZero, 1, 1))
                    player.AddBuff(BuffID.Blackout, 900);

            }
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                Terraria.NPC npc = Main.npc[i];
                if (!npc.active || npc.dontTakeDamage || npc.immortal)
                    continue;

                Terraria.Player player = Main.LocalPlayer;
                if (npc.Distance(nukeGroundZero) < 287 * 16)
                    player.ApplyDamageToNPC(npc, 50000, 0, 0, false);
            }
        }
        #endregion

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
            slayerRep = 0;
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
            slayerRep = 0;
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
            tag["slayerRep"] = slayerRep;
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
            slayerRep = tag.GetInt("slayerRep");
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
            writer.Write(slayerRep);
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
            slayerRep = reader.ReadInt32();
        }
    }
}