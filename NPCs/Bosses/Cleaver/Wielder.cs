using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Redemption.Globals;
using Terraria.GameContent;
using System.IO;
using Redemption.BaseExtension;
using Terraria.GameContent.ItemDropRules;
using Redemption.Items.Weapons.HM.Melee;
using Terraria.Audio;
using ReLogic.Content;
using Redemption.UI.ChatUI;
using Terraria.Localization;
using Redemption.Globals.NPC;
using Redemption.Textures;

namespace Redemption.NPCs.Bosses.Cleaver
{
    [AutoloadBossHead]
    public class Wielder : ModNPC
    {
        private static Asset<Texture2D> glowMask;
        private static Asset<Texture2D> boosterAni;
        private static Asset<Texture2D> boosterGlow;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            glowMask = ModContent.Request<Texture2D>(Texture + "_Glow");
            boosterAni = ModContent.Request<Texture2D>(Texture + "_Booster");
            boosterGlow = ModContent.Request<Texture2D>(Texture + "_Booster_Glow");
        }
        public override void Unload()
        {
            glowMask = null;
            boosterAni = null;
            boosterGlow = null;
        }
        public enum ActionState
        {
            Begin,
            Intro,
            Intro2,
            Idle,
            Attacks,
            Death,
            Death2
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[2];

        public ref float AIHost => ref NPC.ai[3];

        public float[] oldrot = new float[5];

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wielder Bot");
            Main.npcFrameCount[NPC.type] = 19;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 32;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 2;
            NPC.lifeMax = 28000;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.chaseable = false;
        }
        private static Texture2D Bubble => CommonTextures.TextBubble_Omega.Value;
        private static readonly SoundStyle voice = CustomSounds.Voice6 with { Pitch = 0.4f };
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.DamageType == DamageClass.Melee)
                modifiers.FinalDamage *= 2.5f;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Redemption().TechnicallyMelee)
                modifiers.FinalDamage *= 2.5f;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0.5f;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordRemote>()));
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.LifeDrain, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }

        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            return !NPC.AnyNPCs(ModContent.NPCType<OmegaCleaver>()) && (!player.active || player.dead || Main.dayTime || (AIState == ActionState.Death2 && NPC.ai[2] >= 340));
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackNumber);
            writer.Write(cooldown);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackNumber = reader.ReadInt32();
            cooldown = reader.ReadInt32();
        }

        public int aniType;
        public int boosterFrame;
        public int cooldown;
        private int AttackNumber;
        private bool Funny;

        void AttackChoice()
        {
            int attempts = 0;
            while (attempts == 0)
            {
                if (CopyList == null || CopyList.Count == 0)
                    CopyList = new List<int>(AttackList);
                ID = CopyList[Main.rand.Next(0, CopyList.Count)];
                CopyList.Remove(ID);
                NPC.netUpdate = true;

                if (ID == 3 && AttackNumber <= 5)
                    continue;

                AttackNumber++;
                attempts++;
            }
        }

        public List<int> AttackList = new() { 0, 1, 2, 3, 4, 5, 6, 7 };
        public List<int> CopyList = null;
        public int ID { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }

        public override void AI()
        {
            if (NPC.DespawnHandler())
                return;
            Player player = Main.player[NPC.target];
            if (AIState >= ActionState.Idle && AIState != ActionState.Death && AIState != ActionState.Death2)
                NPC.dontTakeDamage = false;
            else
                NPC.dontTakeDamage = true;

            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (cooldown < 0)
                cooldown = 0;
            Vector2 SwingPos = new(150 * NPC.RightOfDir(player), -20);
            Vector2 AwayPos = new(500 * NPC.RightOfDir(player), -40);

            if (!player.active || player.dead)
                return;

            switch (AIState)
            {
                case ActionState.Begin:
                    {
                        if (AITimer++ == 0 && Main.rand.NextBool(50))
                            Funny = true;

                        ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 0);
                        if (AITimer == 1)
                        {
                            aniType = 3;
                            NPC.velocity.Y -= 8;
                        }

                        if (NPC.Center.Y < player.Center.Y + 80 || NPC.ai[2] > 200)
                        {
                            NPC.LookAtEntity(player);
                            aniType = 0;
                            NPC.velocity *= 0.94f;
                            if (NPC.velocity.Length() < 3)
                            {
                                NPC.velocity *= 0;
                                NPC.netUpdate = true;
                                AIState = ActionState.Intro;
                                AITimer = 0;
                            }
                        }
                    }
                    break;
                case ActionState.Intro:
                    AITimer++;
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 0);
                    if (Funny && !Main.dedServ)
                    {
                        if (AITimer == 60)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.WielderBot.Funny1"), Colors.RarityRed, Color.DarkRed, voice, .03f, 2f, 0, false, null, Bubble))
                                 .Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.WielderBot.Funny2"), Colors.RarityRed, Color.DarkRed, voice, .03f, 2f, .5f, true, null, Bubble, endID: 1));
                            chain.OnEndTrigger += Chain_OnEndTrigger;
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer > 600)
                        {
                            if (!NPC.AnyNPCs(ModContent.NPCType<OmegaCleaver>()))
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), NPC.spriteDirection == 1 ? (int)NPC.Center.X - 1400 : (int)NPC.Center.X + 1400, (int)NPC.Center.Y + 150, ModContent.NPCType<OmegaCleaver>(), ai3: NPC.whoAmI);
                            aniType = 4;
                            AITimer = 0;
                            AIState = ActionState.Intro2;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (AITimer > 60)
                        {
                            if (!NPC.AnyNPCs(ModContent.NPCType<OmegaCleaver>()))
                                RedeHelper.SpawnNPC(NPC.GetSource_FromAI(), NPC.spriteDirection == 1 ? (int)NPC.Center.X - 1400 : (int)NPC.Center.X + 1400, (int)NPC.Center.Y + 150, ModContent.NPCType<OmegaCleaver>(), ai3: NPC.whoAmI);

                            aniType = 4;
                            AITimer = 0;
                            AIState = ActionState.Intro2;
                            NPC.netUpdate = true;
                        }
                    }
                    break;
                case ActionState.Intro2:
                    ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Medium, 1200, 2400, 0);
                    if (AIHost == 1)
                    {
                        AITimer++;
                        if (Funny && AITimer == 1 && !Main.dedServ)
                        {
                            DialogueChain chain = new();
                            chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.WielderBot.Funny3"), Colors.RarityRed, Color.DarkRed, voice, .03f, 2f, .5f, true, null, Bubble));
                            ChatUI.Visible = true;
                            ChatUI.Add(chain);
                        }
                        if (AITimer > (Funny ? 120 : 60))
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < 4; i++)
                                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<WielderOrb>(), 0, Vector2.Zero, NPC.whoAmI, i * 90);
                            }
                            AIHost = 0;
                            AIState = ActionState.Idle;
                            AITimer = 0;
                            NPC.netUpdate = true;
                            if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                        }
                    }
                    break;
                case ActionState.Idle:
                    if (!NPC.AnyNPCs(ModContent.NPCType<OmegaCleaver>()))
                    {
                        AIState = ActionState.Death;
                        NPC.netUpdate = true;
                        break;
                    }
                    NPC.LookAtEntity(player);
                    aniType = 0;
                    NPC.velocity *= .98f;
                    if (AIHost == 2)
                    {
                        AITimer++;
                        if (AITimer == 5)
                        {
                            if (cooldown > 0)
                                cooldown--;
                        }
                        if (AITimer > 60)
                        {
                            AITimer = 0;
                            AIState = ActionState.Attacks;
                            AIHost = 0;
                            AttackChoice();
                            NPC.netUpdate = true;
                        }
                    }
                    break;

                case ActionState.Attacks:
                    if (!NPC.AnyNPCs(ModContent.NPCType<OmegaCleaver>()))
                    {
                        AIState = ActionState.Death;
                        NPC.netUpdate = true;
                        break;
                    }
                    switch (ID)
                    {
                        #region Overhead Swing
                        case 0:
                            NPC.LookAtEntity(player);
                            if (AITimer < 120) { AIHost = 3; }
                            AITimer++;
                            if (AITimer < 80)
                                NPC.Move(SwingPos, 16, 10, true);
                            else
                            {
                                NPC.velocity *= .94f;
                                if (AITimer == 80 && !Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.OODashReady, Main.npc[(int)AIHost].position);
                                if (AITimer == 120)
                                {
                                    aniType = 1;
                                    AIHost = 4;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer == 125)
                                    NPC.Dash(7, false, SoundID.Item1, player.Center);
                                if (AITimer > 150)
                                {
                                    AIHost = 0;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion
                        #region Stab
                        case 1:
                            NPC.LookAtEntity(player);
                            if (AITimer < 80)
                                AIHost = 3; NPC.Move(AwayPos, 16, 10, true);

                            AITimer++;
                            NPC.velocity *= .94f;
                            if (AITimer == 81 && !Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.OODashReady, Main.npc[(int)AIHost].position);
                            if (AITimer == 80)
                            {
                                aniType = 2;
                                AIHost = 5;
                                NPC.netUpdate = true;
                            }
                            if (AITimer == 90)
                                NPC.Dash(12, false, SoundID.Item1, player.Center);
                            if (AITimer > 200)
                            {
                                AIHost = 0;
                                AIState = ActionState.Idle;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion
                        #region Speen
                        case 2:
                            if (NPC.Distance(player.Center) > 700 && NPC.ai[2] == 0)
                            {
                                AITimer = 1;
                                NPC.netUpdate = true;
                            }
                            if (AITimer != 0)
                            {
                                NPC.LookAtEntity(player);
                                if (AITimer < 80)
                                {
                                    AIHost = 3;
                                    NPC.Move(SwingPos, 6, 10, true);
                                }
                                NPC.ai[2]++;
                                if (NPC.ai[2] == 80)
                                {
                                    aniType = 3;
                                    AIHost = 6;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer >= 80)
                                {
                                    NPC.Move(new Vector2(0, 0), 24, 50, true);
                                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                                }
                                if (AITimer > 400)
                                {
                                    NPC.rotation = 0;
                                    aniType = 0;
                                    AIHost = 0;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackChoice();
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion
                        #region Overhead Swing Spin
                        case 3:
                            NPC.LookAtEntity(player);
                            if (AITimer < 120)
                                AIHost = 3;

                            AITimer++;
                            if (AITimer < 80)
                                NPC.Move(SwingPos, 16, 10, true);
                            else
                            {
                                NPC.velocity *= .94f;
                                if (AITimer == 81 && !Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.OODashReady, Main.npc[(int)AIHost].position);

                                if (AITimer == 120)
                                {
                                    aniType = 1;
                                    AIHost = 7;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer == 125)
                                    NPC.Dash(7, false, SoundID.Item1, player.Center);
                                if (AITimer > 180)
                                {
                                    AIHost = 0;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            break;
                        #endregion
                        #region Sword Burst
                        case 4:
                            if (AITimer == 0)
                            {
                                if (cooldown == 0)
                                    AIHost = 20;
                                else
                                    AITimer = -1;
                                NPC.netUpdate = true;
                            }
                            else if (AITimer > 0)
                            {
                                NPC.LookAtEntity(player);
                                if (AITimer < 81)
                                    AIHost = 3;

                                AITimer++;
                                NPC.Move(AwayPos, 9, 10, true);

                                if (AITimer == 81)
                                {
                                    if (!Main.dedServ)
                                        SoundEngine.PlaySound(CustomSounds.OODashReady with { Pitch = -0.9f }, Main.npc[(int)AIHost].position);
                                    aniType = 2;
                                    AIHost = 8;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer > 361)
                                {
                                    cooldown = 2;
                                    AIHost = 0;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackChoice();
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion
                        #region Red Prism
                        case 5:
                            if (AITimer == 0)
                            {
                                if (cooldown == 0)
                                    AIHost = 21;
                                else
                                    AITimer = -1;
                                NPC.netUpdate = true;
                            }
                            else if (AITimer > 0)
                            {
                                NPC.LookAtEntity(player);
                                if (AITimer < 81)
                                    AIHost = 3; NPC.Move(AwayPos, 8, 10, true);

                                AITimer++;
                                if (AITimer == 81)
                                {
                                    aniType = 3;
                                    AIHost = 9;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer >= 81)
                                {
                                    NPC.Move(AwayPos, 10, 10, true);
                                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                                }
                                if (AITimer > 321)
                                {
                                    cooldown = 2;
                                    NPC.rotation = 0;
                                    aniType = 0;
                                    AIHost = 0;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackChoice();
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion
                        #region Blade Pillars
                        case 6:
                            if (AITimer == 0)
                            {
                                if (cooldown == 0)
                                    AIHost = 22;
                                else
                                    AITimer = -1;
                                NPC.netUpdate = true;
                            }
                            else if (AITimer > 0)
                            {
                                NPC.LookAtEntity(player);
                                if (AITimer < 81)
                                    AIHost = 3;

                                AITimer++;
                                NPC.Move(AwayPos, 13, 10, true);
                                if (AITimer == 81)
                                {
                                    aniType = 2;
                                    AIHost = 10;
                                    NPC.netUpdate = true;
                                }
                                if (AITimer >= 800)
                                {
                                    cooldown = 2;
                                    AIHost = 0;
                                    AIState = ActionState.Idle;
                                    AITimer = 0;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                AttackChoice();
                                NPC.netUpdate = true;
                            }
                            break;
                        #endregion
                        #region Twin Blade Slice
                        case 7:
                            NPC.LookAtEntity(player);
                            NPC.Move(AwayPos, 13, 10, true);
                            if (AITimer < 100)
                            {
                                if (AITimer < 40)
                                    AIHost = 3;
                                AITimer++;
                                if (AITimer == 40)
                                {
                                    aniType = 1;
                                    AIHost = 11;
                                    AITimer = 100;
                                    NPC.netUpdate = true;
                                }
                            }
                            if (AITimer == 200)
                            {
                                aniType = 1;
                                NPC.Dash(7, false, SoundID.Item1, player.Center);
                                AITimer = 100;
                            }
                            if (AITimer == 300)
                            {
                                aniType = 2;
                                NPC.Dash(7, false, SoundID.Item1, player.Center);
                                AITimer = 100;
                            }
                            if (AITimer >= 1000)
                            {
                                AIHost = 0;
                                AIState = ActionState.Idle;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                            #endregion
                    }
                    break;
                case ActionState.Death:
                    NPC.LookAtEntity(player);
                    aniType = 0;
                    NPC.velocity *= .96f;
                    NPC.rotation = 0;
                    AITimer = 0;
                    AIState = ActionState.Death2;
                    AIHost = 0;
                    NPC.ai[1] = 0;
                    NPC.netUpdate = true;
                    break;
                case ActionState.Death2:
                    NPC.LookAtEntity(player);
                    if (NPC.ai[2] < 260)
                    {
                        ScreenPlayer.CutsceneLock(player, NPC, ScreenPlayer.CutscenePriority.Low, 1200, 2400, 0);
                    }

                    NPC.velocity *= .96f;
                    AITimer++;
                    if (AITimer == 180)
                    {
                        DialogueChain chain = new();
                        chain.Add(new(NPC, Language.GetTextValue("Mods.Redemption.Cutscene.WielderBot.Defeat"), Colors.RarityRed, Color.DarkRed, voice, .01f, 2f, 0, false, null, Bubble));
                        ChatUI.Visible = true;
                        ChatUI.Add(chain);
                    }
                    if (AITimer > 260)
                    {
                        aniType = 3;
                        NPC.velocity.Y -= 1;
                        if (NPC.timeLeft > 10)
                            NPC.timeLeft = 10;
                    }
                    break;
            }
            if (NPC.ai[1] != 2)
            {
                int dustIndex = Dust.NewDust(new Vector2(NPC.position.X + 3, NPC.position.Y + 16), 10, 2, DustID.LifeDrain, 0, 0, 0, default, 1f);
                Main.dust[dustIndex].noGravity = true;
                Dust dust = Main.dust[dustIndex];
                dust.velocity.Y = 3;
                dust.velocity.X = 0;
            }
        }
        private void Chain_OnEndTrigger(Dialogue dialogue, int ID)
        {
            AITimer = 550;
        }
        public override void FindFrame(int frameHeight)
        {
            switch (aniType)
            {
                case 0:
                    NPC.frameCounter++;
                    if (NPC.velocity.Length() == 0)
                        NPC.frame.Y = 0;
                    else
                        NPC.frame.Y = frameHeight;
                    break;
                case 1: // Swing
                    if (NPC.frame.Y < 2 * frameHeight)
                        NPC.frame.Y = 2 * frameHeight;

                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 4 * frameHeight)
                        {
                            NPC.frame.Y = 0;
                            aniType = 0;
                        }
                    }
                    break;
                case 2: // Stab
                    if (NPC.frame.Y < 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;

                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 9 * frameHeight)
                        {
                            NPC.frame.Y = 0;
                            aniType = 0;
                        }
                    }
                    break;
                case 3: // Speen
                    if (NPC.frame.Y < 10 * frameHeight)
                        NPC.frame.Y = 10 * frameHeight;

                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y >= 18 * frameHeight)
                            NPC.frame.Y = 11 * frameHeight;
                    }
                    break;
                case 4: // Dramatic Entrance
                    if (NPC.frame.Y < 4 * frameHeight)
                        NPC.frame.Y = 4 * frameHeight;

                    NPC.frameCounter++;
                    if (NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 6 * frameHeight)
                            NPC.frame.Y = 6 * frameHeight;
                    }
                    break;
            }
            if (NPC.frameCounter % 5 == 0)
            {
                boosterFrame++;
            }
            if (boosterFrame >= 4)
                boosterFrame = 0;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int num214 = boosterAni.Value.Height / 4;
            int y6 = num214 * boosterFrame;
            spriteBatch.Draw(boosterAni.Value, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, y6, boosterAni.Value.Width, num214)), drawColor, NPC.rotation, new Vector2(boosterAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0);
            spriteBatch.Draw(boosterGlow.Value, NPC.Center - screenPos, new Rectangle?(new Rectangle(0, y6, boosterAni.Value.Width, num214)), RedeColor.RedPulse, NPC.rotation, new Vector2(boosterAni.Value.Width / 2f, num214 / 2f), NPC.scale, effects, 0);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask.Value, NPC.Center - screenPos, NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/RedEyeFlare").Value;
            if (AIState == ActionState.Intro2 && AIHost == 1 && AITimer < 60)
            {
                Vector2 position = NPC.Center - screenPos + new Vector2(NPC.spriteDirection == 1 ? 5 : -5, -10);
                RedeDraw.DrawEyeFlare(spriteBatch, ref NPC.ai[2], position, Color.Red, NPC.rotation, 1, 0, flare);
            }
        }
    }
}