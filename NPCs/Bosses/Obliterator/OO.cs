using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Armor.Vanity;
using Redemption.Items.Usable;
using Redemption.Buffs.Debuffs;
using System.Collections.Generic;
using System.IO;
using Redemption.Items.Weapons.PostML.Ranged;
using Terraria.DataStructures;
using Redemption.Buffs.NPCBuffs;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.Obliterator
{
    [AutoloadBossHead]
    public class OO : ModNPC
    {
        public float[] oldrot = new float[5];

        public enum ActionState
        {
            Intro,
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Omega Obliterator");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Confused,
                    BuffID.Poisoned,
                    BuffID.Venom,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 181000;
            NPC.damage = 280;
            NPC.defense = 80;
            NPC.knockBackResist = 0f;
            NPC.width = 100;
            NPC.height = 160;
            NPC.npcSlots = 10f;
            NPC.SpawnWithHigherTime(30);
            NPC.value = Item.buyPrice(0, 20, 0, 0);
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit42;
            NPC.DeathSound = SoundID.NPCDeath14;
            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/BossOmega2");
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LidenBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("An autonomous war machine mostly of Girus' own design with many integrated weapon systems, such as literal finger guns and an advanced heat dispersion system in the form of a giant beam, capable of obliterating anyone it engulfs - Also where Obliterator got its name from.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.MissileExplosion, NPC.position);
                RedeDraw.SpawnExplosion(NPC.Center, Color.OrangeRed);

                for (int i = 0; i < 80; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, 0f, 0f, 100, default, 3f);
                    Main.dust[dustIndex].velocity *= 1.9f;
                }
                for (int i = 0; i < 45; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SparksMech, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity *= 1.8f;
                }
                for (int i = 0; i < 25; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity *= 1.8f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void OnKill()
        {
            if (!RedeBossDowned.downedVlitch3)
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OO_GirusTalk>(), 0, 0, Main.myPlayer);

            NPC.SetEventFlagCleared(ref RedeBossDowned.downedVlitch3, -1);
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0.8;
            return true;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        public override bool CheckActive()
        {
            Player player = Main.player[NPC.target];
            return !player.active || player.dead || Main.dayTime;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(repeat);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                repeat = reader.ReadInt32();
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            ArmFrameY[0] = 2;
            ArmFrameY[1] = 1;
            ArmRot[0] = MathHelper.PiOver2 + (NPC.spriteDirection == -1 ? 0 : MathHelper.Pi);
            ArmRot[1] = 0f;
            HandsFrameY[0] = 2;
            HandsFrameY[1] = 2;
        }
        public Vector2 MoveVector2;

        public int frameCounters;
        public int repeat;
        public bool BeamAnimation;
        void AttackChoice()
        {
        }
        public override void AI()
        {
            Player player = Main.player[RedeHelper.GetNearestAlivePlayer(NPC)];
            NPC.LookAtEntity(player);
            //DespawnHandler();
            Lighting.AddLight(NPC.Center, 0.7f, 0.4f, 0.4f);

            float RotFlip = NPC.spriteDirection == -1 ? 0 : MathHelper.Pi;
            Vector2 DefaultPos = new(player.Center.X - (240 * NPC.spriteDirection), player.Center.Y - 80);

            switch (AIState)
            {
                case ActionState.Intro:
                    switch (TimerRand)
                    {
                        case 0:
                            ArmRot[0] = MathHelper.PiOver2 + (NPC.spriteDirection == -1 ? 0 : MathHelper.Pi);
                            NPC.LookAtEntity(player);
                            AITimer++;
                            if (NPC.DistanceSQ(DefaultPos) < 100 * 100 || AITimer > 200)
                            {
                                AITimer = 0;
                                if (RedeBossDowned.oblitDeath == 2)
                                    TimerRand = 2;
                                else
                                    TimerRand = 1;
                                NPC.netUpdate = true;
                            }
                            else
                                NPC.MoveToVector2(DefaultPos, 11f);
                            break;
                        case 1:
                            NPC.velocity *= 0.96f;
                            if (AITimer++ >= 30)
                            {
                                NPC.velocity *= 0f;
                                player.RedemptionScreen().ScreenFocusPosition = NPC.Center;
                                player.RedemptionScreen().lockScreen = true;
                            }
                            if (AITimer == 120)
                            {
                                HandsFrameY[0] = 0;
                                ArmFrameY[0] = 1;
                                HeadFrameY = 1;
                                CombatText.NewText(NPC.getRect(), Colors.RarityRed, "Yo.", true, false);
                            }
                            if (AITimer >= 120 && AITimer < 188)
                                ArmRot[0].SlowRotation(MathHelper.PiOver2 + (-1 * NPC.spriteDirection) + RotFlip, MathHelper.Pi / 50);
                            else if (AITimer >= 188 && AITimer < 350)
                            {
                                HeadFrameY = 0;
                                HandsFrameY[0] = 2;
                                ArmRot[0].SlowRotation(0, MathHelper.Pi / 20);
                            }
                            else
                                ArmRot[0] = MathHelper.PiOver2 + RotFlip;
                            if (AITimer < 190 || AITimer > 370)
                                NPC.LookAtEntity(player);
                            if (AITimer == 190 && player.active && !player.dead)
                            {
                                BeamAnimation = true;
                                // Beam Animation
                            }
                            if (AITimer == 238)
                                player.GetModPlayer<ScreenPlayer>().Rumble(112, 20);
                            if (AITimer == 350)
                            {
                                ArmFrameY[0] = 2;
                                BeamAnimation = false;
                                // Beam End
                            }
                            if (AITimer == 400)
                            {
                                if (RedeBossDowned.oblitDeath == 1)
                                    CombatText.NewText(NPC.getRect(), Colors.RarityRed, "I guess I can't fool you twice, huh.", true, false);
                                else
                                    CombatText.NewText(NPC.getRect(), Colors.RarityRed, "So much for a surprise attack...", true, false);
                            }
                            if (AITimer == 580)
                            {
                                HeadFrameY = 2;
                                CombatText.NewText(NPC.getRect(), Colors.RarityRed, "Hang on, I got a call from Girus.", true, false);
                            }
                            if (AITimer == 740)
                                CombatText.NewText(NPC.getRect(), Colors.RarityRed, "'I wasted too much energy too quickly?'", true, false);
                            if (AITimer == 840)
                                CombatText.NewText(NPC.getRect(), Colors.RarityRed, "'I'm an idiot?'", true, false);
                            if (AITimer == 940)
                                CombatText.NewText(NPC.getRect(), Colors.RarityRed, "You're scrapping my personality drive?", true, false);
                            if (AITimer == 1140)
                            {
                                HeadFrameY = 0;
                                CombatText.NewText(NPC.getRect(), Colors.RarityRed, "Ah well, request accepted...", true, false);
                            }
                            if (AITimer == 1380)
                                CombatText.NewText(NPC.getRect(), Colors.RarityRed, "REBOOTING SYSTEMS... GENERATING BARRIER...", true, false);
                            if (AITimer > 1500)
                            {
                                if (!Main.dedServ)
                                    SoundEngine.PlaySound(CustomSounds.LabSafeS, NPC.position);
                                for (int i = 0; i < 100; i++)
                                {
                                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, Scale: 1.5f);
                                    Main.dust[dustIndex].velocity *= 1.9f;
                                }
                                RedeBossDowned.oblitDeath = 2;
                                AIState = ActionState.Begin;
                                AITimer = 0;
                                NPC.netUpdate = true;
                            }
                            break;
                    }
                    break;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (BeamAnimation)
            {
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 5 * frameHeight)
                        NPC.frame.Y = 5 * frameHeight;
                }
            }
            else
            {
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y -= frameHeight;
                    if (NPC.frame.Y < 0)
                        NPC.frame.Y = 0;
                }
            }

            LegFrameY = 2 + (int)(-NPC.velocity.Y / 6);
            LegFrameY = (int)MathHelper.Clamp(LegFrameY, 0, 4);
            NPC.rotation = NPC.velocity.X * 0.01f;
        }
        public void Dash(int speed, bool directional)
        {
            Player player = Main.player[NPC.target];
            SoundEngine.PlaySound(SoundID.Item74, NPC.position);
            if (directional)
                NPC.velocity = NPC.DirectionTo(player.Center) * speed;
            else
                NPC.velocity.X = player.Center.X > NPC.Center.X ? speed : -speed;
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.velocity *= 0.96f;
                if (NPC.ai[0] == 1 && AITimer > 190 && RedeBossDowned.oblitDeath == 0)
                {
                    RedeBossDowned.oblitDeath = 1;
                    AITimer = 0;
                    NPC.ai[3] = 0;
                    NPC.ai[0] = 6;
                }
                else if (NPC.ai[0] == 6)
                {
                    AITimer++;
                    if (AITimer == 100)
                        CombatText.NewText(NPC.getRect(), Colors.RarityRed, "Alright, target eliminated.", true, false);
                    if (AITimer == 190)
                        CombatText.NewText(NPC.getRect(), Colors.RarityRed, "Returning to base...", true, false);
                    if (AITimer > 190)
                    {
                        NPC.velocity.Y -= 1;
                        if (NPC.timeLeft > 10)
                            NPC.timeLeft = 10;
                    }
                }
                else
                {
                    if (NPC.ai[0] != 7)
                    {
                        if (RedeBossDowned.oblitDeath == 2)
                            CombatText.NewText(NPC.getRect(), Colors.RarityRed, "TARGET OBLITERATED... RETURNING TO GIRUS...", true, false);
                        else
                            CombatText.NewText(NPC.getRect(), Colors.RarityRed, "Target eliminated...", true, false);
                        AITimer = 0;
                        NPC.ai[0] = 7;
                    }
                    if (NPC.ai[0] == 7 && ++AITimer > 120)
                        NPC.velocity.Y -= 1;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                }
                return;
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }
        private int LegFrameY;
        private int[] ArmFrameY = new int[2];
        private float[] ArmRot = new float[2];
        private int ArmRFrameY;
        private int[] HandsFrameY = new int[2];
        private int HeadFrameY;
        private readonly int[] HandArmX = new int[] { -18, 0, 6 };
        private readonly int[] HandArmY = new int[] { 8, 0, -14 };
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D armB = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arm_Back").Value;
            Texture2D armF = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arm_Front").Value;
            Texture2D armR = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Arm_Rockets").Value;
            Texture2D hands = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Hands").Value;
            Texture2D head = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Head").Value;
            Texture2D headGlow = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Head_Glow").Value;
            Texture2D legs = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Legs").Value;
            Texture2D thruster = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Gigapora_ThrusterBlue").Value;
            float thrusterScaleX = MathHelper.Lerp(1.5f, 0.5f, -NPC.velocity.Y / 20);
            thrusterScaleX = MathHelper.Clamp(thrusterScaleX, 0.5f, 1.5f);
            float thrusterScaleY = MathHelper.Clamp(-NPC.velocity.Y / 10, 0.3f, 2f);
            Vector2 p = NPC.Center - screenPos;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    Vector2 value4 = NPC.oldPos[i];
                    spriteBatch.Draw(texture, value4 + NPC.Size / 2f - screenPos + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, NPC.gfxOffY), NPC.frame, RedeColor.VlitchGlowColour * 0.5f, oldrot[i], NPC.frame.Size() / 2f, NPC.scale, effects, 0);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            int armsHeight = armF.Height / 3;
            Rectangle rectArmF = new(0, armsHeight * ArmFrameY[1], armF.Width, armsHeight);
            Rectangle rectArmB = new(0, armsHeight * ArmFrameY[0], armB.Width, armsHeight);
            Vector2 originArms = new(armF.Width / 2f + (-6 * NPC.spriteDirection), armsHeight / 2f - 14);

            int handsHeight = hands.Height / 3;
            Rectangle rectHandF = new(0, handsHeight * HandsFrameY[1], hands.Width / 2, handsHeight);
            Rectangle rectHandB = new(hands.Width / 2, handsHeight * HandsFrameY[0], hands.Width / 2, handsHeight);

            int armRHeight = armR.Height / 4;
            int armRWidth = armR.Width / 3;
            Rectangle rectArmRB = new(armRWidth * ArmFrameY[0], armRHeight * ArmRFrameY, armRWidth, armRHeight);
            Rectangle rectArmRF = new(armRWidth * ArmFrameY[1], armRHeight * ArmRFrameY, armRWidth, armRHeight);

            if (NPC.frame.Y <= 0)
            {
                // Back Arm
                spriteBatch.Draw(armB, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectArmB), NPC.GetAlpha(drawColor), ArmRot[0], originArms, NPC.scale, effects, 0);
                // Back Hand
                spriteBatch.Draw(hands, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectHandB), NPC.GetAlpha(drawColor), ArmRot[0], originArms - new Vector2((NPC.spriteDirection == -1 ? 29 : 12) + (HandArmX[ArmFrameY[0]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[0]]), NPC.scale, effects, 0);
                // Rockets Back
                spriteBatch.Draw(armR, p + new Vector2(NPC.spriteDirection == -1 ? -34 : -10, -13), new Rectangle?(rectArmRB), NPC.GetAlpha(drawColor), ArmRot[0], originArms - new Vector2(NPC.spriteDirection == -1 ? -3 : 14, 0), NPC.scale, effects, 0);
            }
            // Body

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 thrusterOrigin = new(thruster.Width / 2f, thruster.Height / 2f - 20);
            for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
            {
                Vector2 oldPos = NPC.oldPos[i];
                spriteBatch.Draw(thruster, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(NPC.spriteDirection == 1 ? -44 : 0, NPC.rotation) + RedeHelper.PolarVector(4, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(-NPC.velocity.Y / 20, 0, 1), oldrot[i], thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
            }
            spriteBatch.Draw(thruster, p + RedeHelper.PolarVector(30, NPC.rotation) + RedeHelper.PolarVector(NPC.spriteDirection == 1 ? -44 : 0, NPC.rotation) + RedeHelper.PolarVector(4, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(-NPC.velocity.Y / 20, 0, 1), NPC.rotation - MathHelper.PiOver2, thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            spriteBatch.Draw(glow, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            if (NPC.frame.Y <= 0)
            {
                // Head
                int headHeight = head.Height / 3;
                Rectangle rectHead = new(0, headHeight * HeadFrameY, head.Width, headHeight);
                spriteBatch.Draw(head, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), new Rectangle?(rectHead), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -48 : 4, -2), NPC.scale, effects, 0);
                spriteBatch.Draw(headGlow, p + new Vector2(NPC.spriteDirection == 1 ? -44 : 0, 0), new Rectangle?(rectHead), RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2 + new Vector2(NPC.spriteDirection == 1 ? -48 : 4, -2), NPC.scale, effects, 0);
            }
            // Legs
            int legsHeight = legs.Height / 5;
            Rectangle rectLegs = new(0, legsHeight * LegFrameY, legs.Width, legsHeight);
            spriteBatch.Draw(legs, p, new Rectangle?(rectLegs), NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2 + new Vector2(22, -78), NPC.scale, effects, 0);
            if (NPC.frame.Y <= 0)
            {
                // Front Arm
                spriteBatch.Draw(armF, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectArmF), NPC.GetAlpha(drawColor), ArmRot[1], originArms, NPC.scale, effects, 0);
                spriteBatch.Draw(hands, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectHandF), NPC.GetAlpha(drawColor), ArmRot[1], originArms - new Vector2((NPC.spriteDirection == -1 ? 28 : 13) + (HandArmX[ArmFrameY[1]] * -NPC.spriteDirection), 78 + HandArmY[ArmFrameY[1]]), NPC.scale, effects, 0);
                spriteBatch.Draw(armR, p + new Vector2(NPC.spriteDirection == -1 ? 15 : -59, -22), new Rectangle?(rectArmRF), NPC.GetAlpha(drawColor), ArmRot[1], originArms - new Vector2(NPC.spriteDirection == -1 ? -3 : 14, 0), NPC.scale, effects, 0);
            }
            return false;
        }
    }
}