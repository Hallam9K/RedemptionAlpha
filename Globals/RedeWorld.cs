using Redemption.BaseExtension;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.NPCs.Bosses.KSIII.Friendly;
using Redemption.NPCs.Friendly.TownNPCs;
using Redemption.NPCs.Minibosses.Calavia;
using Redemption.Projectiles.Misc;
using Redemption.UI.ChatUI;
using Redemption.WorldGeneration;
using Redemption.WorldGeneration.Misc;
using SubworldLibrary;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using static Redemption.Globals.RedeNet;

namespace Redemption.Globals
{
    public class RedeWorld : ModSystem
    {
        public static bool blobbleSwarm;
        public static int blobbleSwarmTimer;
        public static int blobbleSwarmCooldown;
        public static int DayNightCount;
        public static bool SkeletonInvasion;
        public static bool spawnSkeletonInvasion;
        public static bool spawnKeeper;
        public static float RotTime;
        public static bool labSafe;
        public static int labSafeMessageTimer;
        public static bool[] omegaTransmitReady = new bool[3];
        public static bool apidroidKilled;
        public static bool deadRingerGiven;
        public static bool newbGone;
        public static bool slayerMessageGiven;
        public static bool keycardGiven;
        public static bool[] spawnCleared = new bool[5];
        public static bool wastelandMessage;
        public static bool alignmentGiven;

        private static int alignment;

        /// <summary>
        /// <br> The alignment. </br>
        /// <br> Setting it syncs automatically, can be set on clients or server. You must make sure that only one of them calls it. </br>
        /// <br> You can do <c>Alignment += 0</c> just to display the <c>"+0"</c> pop-up text. </br>
        /// </summary>
        public static int Alignment
        {
            get => alignment;
            set => SetAlignment(value, sync: true);
        }

        #region Nuke Shenanigans
        public static int nukeTimer = 1800;
        public static bool nukeCountdownActive = false;
        public static Vector2 nukeGroundZero = Vector2.Zero;
        private static readonly int nukeFireballRadius = 287;
        #endregion

        #region Subworld Updates
        public override void PreUpdateWorld()
        {
            if (SubworldSystem.AnyActive<Redemption>() && !SubworldSystem.IsActive<PlaygroundSub>())
            {
                Wiring.UpdateMech();
                Liquid.skipCount++;
                TileEntity.UpdateStart();
                foreach (TileEntity te in TileEntity.ByID.Values)
                    te.Update();
                TileEntity.UpdateEnd();
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

                    tile?.RandomUpdate(i, j);
                }
            }
        }
        #endregion

        public override void PostUpdateWorld()
        {
            if (SubworldSystem.Current != null)
                return;

            if (Main.time == 1)
            {
                DayNightCount++;
                if (RedeQuest.calaviaVar >= 3 && RedeQuest.calaviaVar < 20 && RedeBossDowned.downedCalavia && !Terraria.NPC.AnyNPCs(ModContent.NPCType<Calavia_NPC>()))
                {
                    Vector2 gathicPortalPos = new((RedeGen.gathicPortalVector.X + 47) * 16, (RedeGen.gathicPortalVector.Y + 20) * 16 + 8);
                    LabArea.SpawnNPCInWorld(gathicPortalPos, ModContent.NPCType<Calavia_NPC>());
                }
            }

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
                                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(175, 75, 255));
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue(status), new Color(175, 75, 255));
                        }
                    }
                }
                if (!Main.dayTime && spawnSkeletonInvasion && Main.time > 1)
                {
                    string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Event.SkeletonParty2");
                    if (WorldGen.spawnEye || Main.bloodMoon || WorldGen.spawnHardBoss > 0)
                        status = Language.GetTextValue("Mods.Redemption.StatusMessage.Event.SkeletonParty3");
                    else
                        SkeletonInvasion = true;

                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(175, 75, 255));
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), new Color(175, 75, 255));

                    spawnSkeletonInvasion = false;
                    SyncData();
                }
            }
            if (SkeletonInvasion && (Main.time >= 16200 || Main.dayTime))
            {
                string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Event.SkeletonParty4");
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(175, 75, 255));
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), new Color(175, 75, 255));

                RedeBossDowned.downedSkeletonInvasion = true;
                SkeletonInvasion = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            #endregion

            #region Keeper Summoning
            if (!Main.dayTime && Terraria.NPC.downedBoss1 && !Main.hardMode && !Main.IsFastForwardingTime())
            {
                if (Main.time == 1 && !WorldGen.spawnEye && !spawnSkeletonInvasion)
                {
                    if (!RedeBossDowned.downedKeeper)
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
                        if (check && Main.rand.NextBool(4))
                        {
                            spawnKeeper = true;

                            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Other.Keeper1");
                            if (Main.netMode == NetmodeID.Server)
                                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(50, 255, 130));
                            else if (Main.netMode == NetmodeID.SinglePlayer)
                                Main.NewText(Language.GetTextValue(status), new Color(50, 255, 130));
                        }
                    }
                }
                if (spawnKeeper && Main.time > 4860)
                {
                    for (int k = 0; k < Main.maxPlayers; k++)
                    {
                        Terraria.Player player = Main.player[k];
                        if (!player.active || player.dead || player.position.Y >= Main.worldSurface * 16.0)
                            continue;

                        int type = ModContent.NPCType<Keeper>();

                        if (!RedeBossDowned.downedKeeper && !Terraria.NPC.AnyNPCs(type))
                        {
                            Terraria.NPC.SpawnOnPlayer(player.whoAmI, type);
                        }
                        spawnKeeper = false;
                        break;
                    }
                }
            }
            #endregion

            #region King Slayer III assist Moonlord
            if (!NPC.downedMoonlord && NPC.MoonLordCountdown > 0 && NPC.MoonLordCountdown == NPC.MaxMoonLordCountdown / 2 && RedeQuest.slayerRep >= 2 && !NPC.AnyNPCs(NPCType<KS3_Friendly>()) && !NPC.AnyNPCs(NPCType<KS3>()))
            {
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    Player player = Main.player[k];
                    if (!player.active || player.position.Y >= Main.worldSurface * 16.0)
                        continue;

                    int type = NPCType<KS3_Friendly>();

                    if (!NPC.AnyNPCs(type))
                    {
                        Vector2 newPos = new(Main.rand.Next(-400, -250), Main.rand.Next(-200, 50));
                        LabArea.SpawnNPCInWorld(player.Center + newPos, NPCType<KS3_Friendly>());
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

            if (!wastelandMessage && RedeBossDowned.nukeDropped && Terraria.NPC.downedMechBossAny)
            {
                wastelandMessage = true;
                SyncData();

                string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.WastelandGrow");
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), new Color(50, 255, 130));
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), new Color(50, 255, 130));
            }

            if (Terraria.NPC.downedMechBoss1 && Terraria.NPC.downedMechBoss2 && Terraria.NPC.downedMechBoss3 && !labSafe)
            {
                if (labSafeMessageTimer++ >= 300)
                {
                    string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabOpen");
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.Cyan);

                    SoundStyle s = CustomSounds.LabSafeS with { Volume = 0.6f };
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(s);

                    labSafe = true;
                    SyncData();
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
                for (int i = 0; i < 6; i++)
                    ConversionHandler.GenWasteland(radiusLeft, radiusRight, radiusDown, ConversionHandler.WastelandCenter, ConversionHandler.Radius);
            }
        }

        public static void OmegaTransmitterMessage()
        {
            LocalizedText status = Language.GetText("Mods.Redemption.StatusMessage.Progression.OmegaCall");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(status.ToNetworkText(), Color.IndianRed);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(status.Value, Color.IndianRed);
        }

        #region Warhead Countdown
        public static void UpdateNukeCountdown()
        {
            if (!nukeCountdownActive)
            {
                nukeTimer = 1800;
                return;
            }
            else if (nukeGroundZero == Vector2.Zero)
            {
                nukeCountdownActive = false;
                return;
            }
            else
            {
                --nukeTimer;
                if (nukeTimer <= -60)
                {
                    SoundEngine.PlaySound(CustomSounds.NukeExplosionFar);
                    Main.LocalPlayer.RedemptionScreen().Rumble(60 * 8, 5);
                    Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 100;

                    RedeHelper.ProjectileExplosion(new EntitySource_TileBreak((int)nukeGroundZero.X, (int)nukeGroundZero.Y), nukeGroundZero, 0, 90, ModContent.ProjectileType<NukeShockwave>(), 1, 80, nukeGroundZero.X, nukeGroundZero.Y);
                    HandleNukeExplosion();
                    WorldGen.KillTile((int)(nukeGroundZero.X / 16), (int)(nukeGroundZero.Y / 16), false, false, true);
                    ConversionHandler.ConvertWasteland(nukeGroundZero, nukeFireballRadius);

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

                if (player.Distance(nukeGroundZero) < nukeFireballRadius * 16)
                {
                    NetworkText nukeDeathReason;

                    WeightedRandom<NetworkText> nukeDeaths = new(Main.rand);
                    nukeDeaths.Add(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Nuclear1", player.name), 5);
                    nukeDeaths.Add(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Nuclear2", player.name, Main.worldName), 5);
                    nukeDeaths.Add(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Nuclear3", player.name), 5);
                    nukeDeaths.Add(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Nuclear4", player.name), 5);
                    nukeDeaths.Add(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Nuclear5", player.name), 5);
                    nukeDeaths.Add(NetworkText.FromKey("Mods.Redemption.StatusMessage.Death.Nuclear6", player.name), 1);

                    nukeDeathReason = nukeDeaths;

                    player.KillMe(PlayerDeathReason.ByCustomReason(nukeDeathReason), 999999, 1);
                }
                if (player.Distance(nukeGroundZero) < nukeFireballRadius * 2 * 16 && Collision.CanHit(player.position, player.width, player.height, nukeGroundZero, 1, 1))
                    player.AddBuff(BuffID.Blackout, 900, quiet: false);

            }
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                Terraria.NPC npc = Main.npc[i];
                if (!npc.active || npc.dontTakeDamage || npc.immortal)
                    continue;

                if (npc.Distance(nukeGroundZero) < nukeFireballRadius * 16)
                    npc.StrikeInstantKill();
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
            labSafe = false;
            apidroidKilled = false;
            deadRingerGiven = false;
            newbGone = false;
            slayerMessageGiven = false;
            keycardGiven = false;
            alignmentGiven = false;
            for (int i = 0; i < spawnCleared.Length; i++)
                spawnCleared[i] = false;
            wastelandMessage = false;
            nukeCountdownActive = false;
            nukeGroundZero = Vector2.Zero;
            nukeTimer = 1800;
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
            if (wastelandMessage)
                lists.Add("wastelandMessage");

            tag["lists"] = lists;
            tag["alignment"] = alignment;
            tag["DayNightCount"] = DayNightCount;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var lists = tag.GetList<string>("lists");

            SkeletonInvasion = lists.Contains("SkeletonInvasion");
            alignment = tag.GetInt("alignment");
            DayNightCount = tag.GetInt("DayNightCount");
            labSafe = lists.Contains("labSafe");
            apidroidKilled = lists.Contains("apidroidKilled");
            deadRingerGiven = lists.Contains("deadRingerGiven");
            newbGone = lists.Contains("newbGone");
            slayerMessageGiven = lists.Contains("slayerMessageGiven");
            keycardGiven = lists.Contains("keycardGiven");
            alignmentGiven = lists.Contains("alignmentGiven");
            wastelandMessage = lists.Contains("wastelandMessage");
        }

        #region Netcode
        /// <summary>
        /// Syncs RedeWorld data. Can be called on clients or server, you must make sure only one of them calls it.
        /// </summary>
        public static void SyncData()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = Redemption.Instance.GetPacket();
                packet.Write((byte)ModMessageType.SyncRedeWorldFromClient);
                ModContent.GetInstance<RedeWorld>().NetSend(packet);
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

            ModContent.GetInstance<RedeWorld>().NetReceive(reader);
            NetMessage.SendData(MessageID.WorldData, ignoreClient: sender);
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
            var flags2 = new BitsByte();
            flags2[0] = wastelandMessage;
            flags2[1] = nukeCountdownActive;
            writer.Write(flags2);

            writer.Write(DayNightCount);

            writer.Write(nukeTimer);
            writer.WriteVector2(nukeGroundZero);
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
            BitsByte flags2 = reader.ReadByte();
            wastelandMessage = flags2[0];
            nukeCountdownActive = flags2[2];

            DayNightCount = reader.ReadInt32();

            nukeTimer = reader.ReadInt32();
            nukeGroundZero = reader.ReadVector2();
        }
        #endregion

        #region Alignment logic
        /// <summary>
        /// <br> Sets the world alignment to a new value and syncs it, also displaying the specified popup text color. Normally you want to offset the current value. </br>
        /// <br> You can offset the property instead, for simplicity: (<c><see cref="Alignment"/> += 2</c>), where the floating text color defaults to <see cref="Color.Gold"/>.</br>
        /// <br> Can be called on clients or servers, you must make sure that only one of them calls it.</br>
        /// </summary>
        /// <param name="newValue"> The new alignment value. </param>
        /// <param name="floatingTextColor"> The floating text color, defaults to <see cref="Color.Gold"/>. Use <see cref="Color.Transparent"/> if you want to hide it. </param>
        /// <param name="sync"> Whether to sync this change, leave true except in <c>HandlePacket</c> </param>
        /// <param name="ignoreClient"> Don't send the alignment sync to a particular client, leave -1 except in <c>HandlePacket</c> </param>
        public static void SetAlignment(int newValue, Color? floatingTextColor = null, bool sync = true, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                alignment = newValue;

                if (sync)
                    SyncAlignment(newValue, floatingTextColor, ignoreClient: ignoreClient);
            }
            else
            {
                int valueChange = newValue - alignment;
                alignment = newValue;

                string text = valueChange >= 0 ? $"+{valueChange}" : valueChange.ToString();
                Color color = floatingTextColor ?? Color.Gold;

                if (valueChange == 0)
                {
                    text = "+0";
                    color = floatingTextColor ?? Color.Gray;
                }

                RedeHelper.FloatingTextAllPlayers(text, color, dramatic: true);

                if (Main.netMode == NetmodeID.MultiplayerClient && sync)
                    SyncAlignment(newValue, floatingTextColor);
            }
        }

        // Sends alignment packet
        private static void SyncAlignment(int newValue, Color? floatingTextColor = null, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = Redemption.Instance.GetPacket();
            packet.Write((byte)ModMessageType.SyncAlignment);

            packet.Write(newValue);

            packet.Write(floatingTextColor.HasValue);
            if (floatingTextColor.HasValue) packet.Write(floatingTextColor.Value.PackedValue);

            packet.Send(toClient, ignoreClient);
        }

        // Receive alignment packet, forwarded in SetAlignment
        public static void ReceiveSyncAlignment(BinaryReader reader, int sender)
        {
            int newValue = reader.ReadInt32();

            Color? floatingTextColor = null;
            if (reader.ReadBoolean()) floatingTextColor = new() { PackedValue = reader.ReadUInt32() };

            bool sync = Main.netMode == NetmodeID.Server;
            SetAlignment(newValue, floatingTextColor, sync, ignoreClient: sender);
        }

        // Send the alignment to the freshly joined client.
        // Here, we are NOT hijacking any packet, just sending an extra packet whenever FinishedConnectingToServer is sent
        public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
        {
            if (Main.netMode == NetmodeID.Server && msgType == MessageID.FinishedConnectingToServer && remoteClient >= 0 && remoteClient < 255)
            {
                SyncAlignment(alignment, Color.Transparent, toClient: remoteClient);
            }

            return false; // If this returns true bad things will happen.
        }

        #endregion
    }
}