using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Items.Accessories.PreHM;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Placeable.Trophies;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PreHM.Melee;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Friendly;
using Redemption.Textures;
using Redemption.UI;
using Redemption.UI.ChatUI;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.Minibosses.SkullDigger
{
    [AutoloadBossHead]
    public class SkullDigger : ModNPC
    {
        public enum ActionState
        {
            Begin,
            Idle,
            Attacks,
            Death
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public float[] oldrot = new float[5];
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Position = new Vector2(0, 30),
                PortraitPositionYOverride = 8
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 92;
            NPC.friendly = false;
            NPC.damage = 28;
            NPC.defense = 0;
            NPC.lifeMax = 2400;
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.SpawnWithHigherTime(30);
            NPC.npcSlots = 10f;
            if (!NPC.IsABestiaryIconDummy)
                NPC.alpha = 255;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.lavaImmune = true;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SilentCaverns");
        }
        private static Texture2D Bubble => !Main.dedServ ? CommonTextures.TextBubble_Cave.Value : null;
        private static readonly SoundStyle voice = CustomSounds.Voice8 with { Volume = .5f, Pitch = -1f };

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.Redemption.FlavorTextBestiary.SkullDigger"))
            });
        }

        public override void OnKill()
        {
            for (int i = 0; i < 6; i++)
                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LostSoulNPC>(), Main.rand.NextFloat(0f, 0.4f));

            if (!RedeBossDowned.downedSkullDigger)
            {
                RedeWorld.Alignment--;
                ChaliceAlignmentUI.BroadcastDialogue(NetworkText.FromKey("Mods.Redemption.UI.Chalice.SkullDiggerDefeat"), 300, 30, 0, Color.DarkGoldenrod);
            }
            NPC.SetEventFlagCleared(ref RedeBossDowned.downedSkullDigger, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<SkullDiggerRelic>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<RaggedPatch>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullDiggerTrophy>(), 10));

            npcLoot.Add(ItemDropRule.ByCondition(new TeddyCondition(), ModContent.ItemType<AbandonedTeddy>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullDiggerFlail>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullDiggerMask>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GraveSteelShards>(), 1, 20, 40));
            npcLoot.Add(ItemDropRule.ByCondition(new LostSoulCondition(), ModContent.ItemType<LostSoul>(), 1, 6, 6));
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ID);
            writer.Write(FloatTimer);

            writer.Write(NPC.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ID = reader.ReadInt32();
            FloatTimer = reader.ReadSingle();

            NPC.dontTakeDamage = reader.ReadBoolean();
        }

        void AttackChoice()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                attempts++;
            }
        }

        private Vector2 origin;
        public bool KeeperSpawn;

        public List<int> AttackList = new() { 0, 1, 2 };
        public List<int> CopyList = null;

        public int ID;
        private float FloatTimer;

        public readonly Vector2 modifier = new(0, -240);

        private bool spawned;

        public override void AI()
        {
            if (!spawned)
            {
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(CustomSounds.SpookyNoise, NPC.position);

                    if (NPC.AnyNPCs(ModContent.NPCType<Keeper>()))
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossKeeper");
                }
                spawned = true;
            }

            Vector2 text = new Vector2(NPC.Center.X, NPC.position.Y - 140) - Main.screenPosition;
            if (MoRDialogueUI.Visible)
                RedeSystem.Instance.DialogueUIElement.TextPos = text;
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!RedeHelper.AnyProjectiles(ModContent.ProjectileType<SkullDigger_FlailBlade>()))
                NPC.Shoot(NPC.Center, ModContent.ProjectileType<SkullDigger_FlailBlade>(), NPC.damage, Vector2.Zero, NPC.whoAmI);

            if (NPC.DespawnHandler(1))
                return;

            if (AIState != ActionState.Death && AIState != ActionState.Attacks)
                NPC.LookAtEntity(player);

            NPC.position.Y += (float)Math.Sin(FloatTimer++ / 15) / 3;

            NPC.netOffset *= 0f;

            switch (AIState)
            {
                case ActionState.Begin:
                    switch (TimerRand)
                    {
                        case 0:
                            if (AITimer++ == 0)
                            {
                                TitleCard.BroadcastTitle(NetworkText.FromKey("Mods.Redemption.TitleCard.SkullDigger.Name"), 60, 90, 0.8f, Color.LightCyan, NetworkText.FromKey("Mods.Redemption.TitleCard.SkullDigger.Modifier"));

                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.SpookyNoise, NPC.position);
                                if (!NPC.AnyNPCs(ModContent.NPCType<Keeper>()) && Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y);
                                }

                                NPC.velocity.Y = -6;
                                NPC.dontTakeDamage = true;
                                NPC.netUpdate = true;
                            }
                            if (AITimer > 2)
                                NPC.alpha -= 2;

                            int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                            Main.dust[dustIndex].velocity.Y -= 10f;
                            Main.dust[dustIndex].velocity.X *= 0f;
                            Main.dust[dustIndex].noGravity = true;

                            NPC.velocity *= 0.96f;
                            if (NPC.alpha <= 0)
                            {
                                TimerRand = 1;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        case 1:
                            AITimer++;
                            if (NPC.AnyNPCs(ModContent.NPCType<Keeper>()))
                            {
                                KeeperSpawn = true;
                                if (AITimer == 40)
                                {
                                    string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.Fight1");
                                    string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.Fight2");
                                    string s3 = Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.Fight3");
                                    DialogueChain chain = new();
                                    chain.Add(new(NPC, s1, Color.LightCyan, Color.Gray, voice, .06f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                         .Add(new(NPC, s2, Color.LightCyan, Color.Gray, voice, .06f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                         .Add(new(NPC, s3, Color.LightCyan, Color.Gray, voice, .06f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                                    chain.OnEndTrigger += Chain_OnEndTrigger;
                                    ChatUI.Visible = true;
                                    ChatUI.Add(chain);
                                }
                                if (AITimer >= 2000)
                                {
                                    NPC.dontTakeDamage = false;
                                    AITimer = 0;
                                    TimerRand = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                if (AITimer == 40)
                                {
                                    string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.Fight4");
                                    string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.Fight3");
                                    DialogueChain chain = new();
                                    chain.Add(new(NPC, s1, Color.LightCyan, Color.Gray, voice, .06f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                         .Add(new(NPC, s2, Color.LightCyan, Color.Gray, voice, .06f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                                    chain.OnEndTrigger += Chain_OnEndTrigger;
                                    ChatUI.Visible = true;
                                    ChatUI.Add(chain);
                                }
                                if (AITimer >= 1000)
                                {
                                    NPC.dontTakeDamage = false;
                                    AITimer = 0;
                                    TimerRand = 0;
                                    AIState = ActionState.Idle;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                    }
                    break;
                case ActionState.Idle:
                    NPC.Move(new Vector2(-160 * NPC.spriteDirection, -30), 2, 20, true);
                    switch (TimerRand)
                    {
                        case 0:
                            AITimer++;
                            if (AITimer >= 5)
                                NPC.alpha += 5;
                            if (NPC.alpha >= 255)
                            {
                                AITimer = 0;
                                NPC.velocity *= 0f;
                                TimerRand = 1;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    NPC.position = new Vector2(Main.rand.NextBool(2) ? player.Center.X - 180 : player.Center.X + 180, player.Center.Y - 30);
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        case 1:
                            NPC.alpha -= 5;
                            if (NPC.alpha <= 0)
                            {
                                NPC.alpha = 0;
                                if (AITimer++ >= (Main.masterMode ? 60 : 120))
                                {
                                    AttackChoice();
                                    AITimer = 0;
                                    TimerRand = 0;
                                    AIState = ActionState.Attacks;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                    }
                    break;
                case ActionState.Attacks:
                    switch (ID)
                    {
                        #region Flail Throw
                        case 0:
                            NPC.LookAtEntity(player);
                            NPC.Move(new Vector2(-200 * NPC.spriteDirection, -30), 2, 20, true);
                            if (AITimer >= 1)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Soul Charge
                        case 1:
                            AITimer++;
                            if (AITimer < 100)
                            {
                                NPC.LookAtEntity(player);
                                NPC.MoveToVector2(new Vector2(player.Center.X - 200 * NPC.spriteDirection, player.Center.Y - 70), 3);
                                for (int i = 0; i < 2; i++)
                                {
                                    Dust dust2 = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.DungeonSpirit, 1);
                                    dust2.velocity = -NPC.DirectionTo(dust2.position);
                                    dust2.noGravity = true;
                                }
                                Vector2 vector;
                                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                                vector.X = (float)(Math.Sin(angle) * 150);
                                vector.Y = (float)(Math.Cos(angle) * 150);
                                Dust dust = Main.dust[Dust.NewDust(NPC.Center + vector + new Vector2(0, 50), 2, 2, ModContent.DustType<DustSpark2>(), newColor: new Color(255, 255, 255, 0), Scale: 1f)];
                                dust.noGravity = true;
                                dust.velocity = dust.position.DirectionTo(NPC.Center + new Vector2(0, 50)) * 3f;
                                origin = player.Center;
                            }
                            if (AITimer >= 100 && AITimer < 120)
                            {
                                NPC.velocity.Y = 0;
                                NPC.velocity.X = -0.1f * NPC.spriteDirection;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity = MathHelper.Max(Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity, 3);

                                if (AITimer % 2 == 0)
                                {
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<KeeperSoulCharge>(), (int)(NPC.damage * 1.4f), RedeHelper.PolarVector(Main.rand.NextFloat(7, 10), (origin - NPC.Center).ToRotation()), SoundID.NPCDeath52 with { Volume = .5f }, 2);
                                }
                            }
                            if (AITimer >= 120)
                                NPC.velocity *= 0.98f;
                            if (AITimer >= 160)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion

                        #region Flail Speen
                        case 2:
                            NPC.LookAtEntity(player);
                            NPC.Move(new Vector2(-200 * NPC.spriteDirection, -30), 2, 20, true);
                            if (AITimer >= 1)
                            {
                                TimerRand = 0;
                                AITimer = 0;
                                AIState = ActionState.Idle;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.Death:
                    NPC.dontTakeDamage = true;
                    NPC.velocity *= 0.96f;
                    if (NPC.AnyNPCs(ModContent.NPCType<Keeper>()))
                    {
                        if (Main.rand.NextBool(3))
                        {
                            int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                            Main.dust[dustIndex].velocity.Y -= 10f;
                            Main.dust[dustIndex].velocity.X *= 0f;
                            Main.dust[dustIndex].noGravity = true;
                        }
                        if (NPC.alpha < 100)
                            NPC.alpha++;
                        if (NPC.alpha > 100)
                            NPC.alpha--;
                    }
                    else
                    {
                        ScreenPlayer.CutsceneLock(Main.LocalPlayer, NPC, ScreenPlayer.CutscenePriority.Low, 1200, 2400, 1200);
                        AITimer++;

                        if (!Main.dedServ && AITimer == 40)
                        {
                            string s1 = Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.Defeat1");
                            string s2 = Language.GetTextValue("Mods.Redemption.Cutscene.SkullDigger.Defeat2");
                            DialogueChain chain = new();
                            chain.Add(new(NPC, s1, Color.LightCyan, Color.Gray, voice, .06f, 2f, 0, false, bubble: Bubble, modifier: modifier))
                                 .Add(new(NPC, s2, Color.LightCyan, Color.Gray, voice, .06f, 2f, .5f, true, bubble: Bubble, modifier: modifier, endID: 1));
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }

                        if (AITimer >= 500)
                        {
                            NPC.alpha++;

                            int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                            Main.dust[dustIndex].velocity.Y -= 10f;
                            Main.dust[dustIndex].velocity.X *= 0f;
                            Main.dust[dustIndex].noGravity = true;

                            if (NPC.alpha >= 255)
                            {
                                NPC.dontTakeDamage = false;
                                NPC.netUpdate = true;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NPC.StrikeInstantKill();
                            }
                        }
                        else
                        {
                            if (NPC.alpha < 100)
                                NPC.alpha++;
                            if (NPC.alpha > 100)
                                NPC.alpha--;

                            if (Main.rand.NextBool(3))
                            {
                                int dustIndex = Dust.NewDust(NPC.BottomLeft + new Vector2(0, 20), NPC.width, 1, DustID.DungeonSpirit);
                                Main.dust[dustIndex].velocity.Y -= 10f;
                                Main.dust[dustIndex].velocity.X *= 0f;
                                Main.dust[dustIndex].noGravity = true;
                            }
                        }
                    }
                    break;
            }
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 2000;
        }

        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];

            if (Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight))
                return true;
            return AIState != ActionState.Death && AIState != ActionState.Begin;
        }

        public override bool CheckDead()
        {
            if (AIState is ActionState.Death && AITimer > 220)
                return true;
            else
            {
                NPC host = Main.npc[(int)NPC.ai[3]];
                if (NPC.ai[3] >= 0 && host.type == ModContent.NPCType<Keeper>())
                {
                    host.dontTakeDamage = false;
                    host.netUpdate = true;
                }

                SoundEngine.PlaySound(SoundID.NPCDeath51, NPC.position);
                NPC.life = 1;
                AITimer = 0;
                AIState = ActionState.Death;
                NPC.netUpdate = true;
                return false;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.05f;
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D HandsTex = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Hands").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int shader = ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex;

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i];
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, oldPos + NPC.Size / 2f - screenPos + new Vector2(0, NPC.gfxOffY), NPC.frame, NPC.GetAlpha(Color.LightCyan) * 0.3f, oldrot[i], NPC.frame.Size() / 2, NPC.scale + 0.1f, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            Rectangle rect = new(0, 0, HandsTex.Width, HandsTex.Height);
            spriteBatch.Draw(HandsTex, NPC.Center - screenPos - new Vector2(14, -32), new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.HasItem(ModContent.ItemType<AbandonedTeddy>()) || RedeBossDowned.keeperSaved)
                return 0;
            float baseChance = SpawnCondition.Cavern.Chance * (!NPC.AnyNPCs(NPC.type) && RedeBossDowned.downedKeeper ? 1 : 0);
            float multiplier = spawnInfo.Player.HasItem(ModContent.ItemType<SorrowfulEssence>()) ? 0.1f : (RedeBossDowned.downedSkullDigger ? 0 : 0.002f);

            return baseChance * multiplier;
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return NPC.GetBestiaryEntryColor();
            return null;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
    }
}
