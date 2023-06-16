using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Globals.Player;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Friendly;
using Redemption.Projectiles.Misc;
using Redemption.UI.ChatUI;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;
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
        #region Soulless Subworld
        public override void PreUpdateWorld()
        {
            if (SubworldSystem.IsActive<SoullessSub>())
            {
                Wiring.UpdateMech();
                Liquid.skipCount++;
                if (Liquid.skipCount > 1)
                {
                    Liquid.UpdateLiquid();
                    Liquid.skipCount = 0;
                }
                for (int num = 0; num < 20; num++)
                {
                    int i = Main.rand.Next(10, 1800 - 10);
                    int j = Main.rand.Next(10, 1800 - 10);
                    ModTile tile = TileLoader.GetTile(Main.tile[i, j].TileType);

                    if (tile != null)
                        tile.RandomUpdate(i, j);
                }
            }
        }
        #endregion

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
        public static float RotTime;
        public static int slayerRep;
        public static bool labSafe;
        public static int labSafeMessageTimer;
        public static bool[] omegaTransmitReady = new bool[3];
        public static bool apidroidKilled;
        public static bool deadRingerGiven;
        public static bool newbGone;
        public static bool slayerMessageGiven;
        public static bool keycardGiven;
        public static bool alignmentGiven;
        public static bool[] spawnCleared = new bool[5];

        #region Nuke Shenanigans
        public static int nukeTimerInternal = 1800;
        public static int nukeTimerShown = 30;
        public static int nukeFireballRadius = 287;
        public static bool nukeCountdownActive = false;
        public static Vector2 nukeGroundZero = Vector2.Zero;
        #endregion

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

            if (SubworldSystem.Current != null)
                return;

            #region Skeleton Invasion
            if (DayNightCount >= 10 && !Main.hardMode && !Main.IsFastForwardingTime())
            {
                if (Main.dayTime && Main.time == 1 && !WorldGen.spawnEye)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (!RedeBossDowned.downedSkeletonInvasion || (DayNightCount > 12 && DayNightCount % 3 == 0 && Main.rand.NextBool(8)))
                        {
                            spawnSkeletonInvasion = true;

                            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Event.SkeletonParty1") + (RedeBossDowned.downedSkeletonInvasion ? Language.GetTextValue("Mods.Redemption.StatusMessage.Event.Again") : "");
                            if (Main.netMode == NetmodeID.Server)
                                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGray);
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue(status), Color.LightGray);
                        }
                    }
                }
                if (!Main.dayTime && spawnSkeletonInvasion && Main.netMode != NetmodeID.MultiplayerClient && Main.time > 1)
                {
                    string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Event.SkeletonParty2");
                    if (WorldGen.spawnEye || Main.bloodMoon || WorldGen.spawnHardBoss > 0)
                        status = Language.GetTextValue("Mods.Redemption.StatusMessage.Event.SkeletonParty3");
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

                string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Event.SkeletonParty4");
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.LightGray);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.LightGray);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            #endregion

            #region Keeper Summoning
            if (!Main.dayTime && Terraria.NPC.downedBoss1 && !Main.hardMode && !Main.IsFastForwardingTime())
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

                            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Keeper1");
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
                            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);

                        spawnKeeper = false;
                        break;
                    }
                }
            }
            #endregion

            #region Fool Leaving
            if (!newbGone && Main.dayTime && RedeBossDowned.downedNebuleus && Terraria.NPC.AnyNPCs(ModContent.NPCType<Newb>()))
            {
                if (Main.time == 1 && Main.rand.NextBool(2))
                {
                    newbGone = true;
                    string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.FoolLeft");
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.SandyBrown);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.SandyBrown);
                }
            }
            #endregion

            if (Terraria.NPC.downedMechBoss1 && Terraria.NPC.downedMechBoss2 && Terraria.NPC.downedMechBoss3 && !labSafe)
            {
                if (labSafeMessageTimer++ >= 300)
                {
                    labSafe = true;

                    string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabOpen");
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.Cyan);

                    SoundStyle s = CustomSounds.LabSafeS with { Volume = 0.6f };
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(s);

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.WorldData);
                }
            }
            if (Terraria.NPC.downedPlantBoss && !omegaTransmitReady[0])
            {
                omegaTransmitReady[0] = true;
                OmegaTransmitterMessage();
            }
            if (Terraria.NPC.downedGolemBoss && !omegaTransmitReady[1])
            {
                omegaTransmitReady[1] = true;
                OmegaTransmitterMessage();
            }
            if (Terraria.NPC.downedMoonlord && keycardGiven && !omegaTransmitReady[2])
            {
                omegaTransmitReady[2] = true;
                OmegaTransmitterMessage();
            }

            int PalebatImpID = Terraria.NPC.FindFirstNPC(ModContent.NPCType<PalebatImp>());
            if (PalebatImpID >= 0 && (Main.npc[PalebatImpID].ModNPC as PalebatImp).shakeTimer > 0)
            {
                if (!Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene.Activate("MoonLordShake", Main.player[Main.myPlayer].position);
                }
                Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].GetShader().UseIntensity((Main.npc[PalebatImpID].ModNPC as PalebatImp).shakeTimer);
            }
            if (Main.player[Main.myPlayer].GetModPlayer<BuffPlayer>().island)
            {
                if (!Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene.Activate("MoonLordShake", Main.player[Main.myPlayer].position);
                }
                Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].GetShader().UseIntensity(0.5f);
            }
            if (Main.player[Main.myPlayer].InModBiome<SoullessBiome>())
            {
                if (!Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].IsActive())
                {
                    Terraria.Graphics.Effects.Filters.Scene.Activate("MoonLordShake", Main.player[Main.myPlayer].position, Array.Empty<object>());
                }
                Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].GetShader().UseIntensity(0.3f);
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

            if (ConversionHandler.GenningWasteland)
            {
                int radiusLeft = (int)(ConversionHandler.WastelandCenter.X / 16f - ConversionHandler.Radius);
                int radiusRight = (int)(ConversionHandler.WastelandCenter.X / 16f + ConversionHandler.Radius);
                int radiusDown = (int)(ConversionHandler.WastelandCenter.Y / 16f + ConversionHandler.Radius);
                if (radiusLeft < 15) { radiusLeft = 15; }
                if (radiusRight > Main.maxTilesX - 15) { radiusRight = Main.maxTilesX - 15; }
                if (radiusDown > Main.maxTilesY - 15) { radiusDown = Main.maxTilesY - 15; }
                for (int i = 0; i < 2; i++)
                    ConversionHandler.GenWasteland(radiusLeft, radiusRight, radiusDown, ConversionHandler.WastelandCenter, ConversionHandler.Radius);
            }
        }

        public static void OmegaTransmitterMessage()
        {
            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.OmegaCall");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.IndianRed);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.IndianRed);

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
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
                    if (!Main.dedServ)
                    {
                        RedeSystem.Instance.DialogueUIElement.DisplayDialogue(nukeTimerShown.ToString(), 40, 8, 1, null, ((30f - nukeTimerShown) / 30f) * 2, Color.Red, Color.Black);
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
                    RedeHelper.ProjectileExplosion(new EntitySource_TileBreak((int)nukeGroundZero.X, (int)nukeGroundZero.Y), nukeGroundZero, 0, 90, ModContent.ProjectileType<NukeShockwave>(), 1, 80, nukeGroundZero.X, nukeGroundZero.Y);
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

        public static void HandleNukeExplosion()
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
                    nukeDeaths.Add(player.name + Language.GetTextValue("Mods.Redemption.StatusMessage.Death.Nuclear1"), 5);
                    nukeDeaths.Add(player.name + Language.GetTextValue("Mods.Redemption.StatusMessage.Death.Nuclear2") + Main.worldName + "", 5);
                    nukeDeaths.Add(player.name + Language.GetTextValue("Mods.Redemption.StatusMessage.Death.Nuclear3"), 5);
                    nukeDeaths.Add(player.name + Language.GetTextValue("Mods.Redemption.StatusMessage.Death.Nuclear4"), 5);
                    nukeDeaths.Add(player.name + Language.GetTextValue("Mods.Redemption.StatusMessage.Death.Nuclear5"), 5);
                    nukeDeaths.Add(player.name + Language.GetTextValue("Mods.Redemption.StatusMessage.Death.Nuclear6"), 1);

                    nukeDeathReason = nukeDeaths;
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.NukeExplosion, player.position);

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
            if (Terraria.NPC.downedPlantBoss)
                omegaTransmitReady[0] = true;
            else
                omegaTransmitReady[0] = false;
            if (Terraria.NPC.downedGolemBoss)
                omegaTransmitReady[1] = true;
            else
                omegaTransmitReady[1] = false;
            if (Terraria.NPC.downedMoonlord)
                omegaTransmitReady[2] = true;
            else
                omegaTransmitReady[2] = false;
        }
        public override void OnWorldUnload()
        {
            omegaTransmitReady[0] = false;
            omegaTransmitReady[1] = false;
            omegaTransmitReady[2] = false;
        }
        public override void ClearWorld()
        {
            Redemption.TrailManager?.ClearAllTrails();
            if (!Main.dedServ)
                AdditiveCallManager.Unload();
            if (!Main.dedServ && ChatUI.Visible)
                ChatUI.Clear();

            alignment = 0;
            DayNightCount = 0;
            SkeletonInvasion = false;
            spawnKeeper = false;
            spawnSkeletonInvasion = false;
            tbotDownedTimer = 0;
            daerelDownedTimer = 0;
            zephosDownedTimer = 0;
            slayerRep = 0;
            labSafe = false;
            apidroidKilled = false;
            deadRingerGiven = false;
            newbGone = false;
            slayerMessageGiven = false;
            keycardGiven = false;
            alignmentGiven = false;
            for (int i = 0; i < spawnCleared.Length; i++)
                spawnCleared[i] = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var lists = new List<string>();

            if (SkeletonInvasion)
                lists.Add("SkeletonInvasion");
            if (labSafe)
                lists.Add("labSafe");
            if (apidroidKilled)
                lists.Add("apidroidKilled");
            if (deadRingerGiven)
                lists.Add("deadRingerGiven");
            if (newbGone)
                lists.Add("newbGone");
            if (slayerMessageGiven)
                lists.Add("slayerMessageGiven");
            if (keycardGiven)
                lists.Add("keycardGiven");
            if (alignmentGiven)
                lists.Add("alignmentGiven");

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
            labSafe = lists.Contains("labSafe");
            apidroidKilled = lists.Contains("apidroidKilled");
            deadRingerGiven = lists.Contains("deadRingerGiven");
            newbGone = lists.Contains("newbGone");
            slayerMessageGiven = lists.Contains("slayerMessageGiven");
            keycardGiven = lists.Contains("keycardGiven");
            alignmentGiven = lists.Contains("alignmentGiven");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = SkeletonInvasion;
            flags[1] = labSafe;
            flags[2] = apidroidKilled;
            flags[3] = deadRingerGiven;
            flags[4] = newbGone;
            flags[5] = slayerMessageGiven;
            flags[6] = keycardGiven;
            flags[7] = alignmentGiven;
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
            labSafe = flags[1];
            apidroidKilled = flags[2];
            deadRingerGiven = flags[3];
            newbGone = flags[4];
            slayerMessageGiven = flags[5];
            keycardGiven = flags[6];
            alignmentGiven = flags[7];

            alignment = reader.ReadInt32();
            DayNightCount = reader.ReadInt32();
            tbotDownedTimer = reader.ReadInt32();
            daerelDownedTimer = reader.ReadInt32();
            zephosDownedTimer = reader.ReadInt32();
            slayerRep = reader.ReadInt32();
        }
    }
}
