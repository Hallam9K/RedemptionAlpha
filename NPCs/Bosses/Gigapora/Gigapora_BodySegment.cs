using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Redemption.Globals;
using Redemption.BaseExtension;
using Terraria.Audio;
using Redemption.Dusts;
using System.IO;
using ReLogic.Content;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigapora_BodySegment : Gigapora
    {
        private static Asset<Texture2D> core;
        private static Asset<Texture2D> coreGlow;
        private static Asset<Texture2D> tail;
        private static Asset<Texture2D> thrusterBlue;
        private static Asset<Texture2D> thrusterOrange;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            core = ModContent.Request<Texture2D>(Texture + "_Core");
            coreGlow = ModContent.Request<Texture2D>(Texture + "_Core_Glow");
            tail = ModContent.Request<Texture2D>(Texture + "_Tail");
            thrusterBlue = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Gigapora_ThrusterBlue");
            thrusterOrange = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Gigapora_ThrusterOrange");
        }
        public override void Unload()
        {
            core = null;
            coreGlow = null;
            tail = null;
            thrusterBlue = null;
            thrusterOrange = null;
        }
        public new float[] oldrot = new float[6];
        public ref float SegmentType => ref NPC.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Gigapora");
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 92;
            NPC.height = 82;
            NPC.dontCountMe = true;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            NPC host = Main.npc[(int)Host];
            if (NPC.life <= 0 && host.ai[0] == 4)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                if (SegmentType <= 0)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/GigaporaGore2").Type);
                else if (SegmentType >= 1 && SegmentType <= 6)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/GigaporaGore3").Type);
                else
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/GigaporaGore4").Type);
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            NPC host = Main.npc[(int)Host];
            return host.ai[0] != 7;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ShootTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ShootTimer = reader.ReadInt32();
        }
        public ref float Host => ref NPC.ai[3];
        public ref float FrameState => ref NPC.ai[0];
        private float shieldAlpha;
        private bool frameLag;
        private int ShootTimer;
        public override bool PreAI()
        {
            for (int k = NPC.oldPos.Length - 1; k > 0; k--)
                oldrot[k] = oldrot[k - 1];
            oldrot[0] = NPC.rotation;

            if (NPC.immortal)
                shieldAlpha += 0.04f;
            else
                shieldAlpha -= 0.04f;
            shieldAlpha = MathHelper.Clamp(shieldAlpha, 0, 0.5f);
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile target = Main.projectile[i];
                if (!target.active || !target.friendly || target.damage <= 0)
                    continue;

                if (target.velocity.Length() == 0 || target.ProjBlockBlacklist() || !NPC.Hitbox.Intersects(target.Hitbox))
                    continue;

                if (NPC.immortal)
                    RedeDraw.SpawnRing(target.Center, Color.Red, 0.13f, 0.7f);
                target.Kill();
            }

            if (FrameState == 2)
            {
                NPC.immortal = false;
            }
            Vector2 chasePosition = Main.npc[(int)NPC.ai[1]].Center;
            Vector2 directionVector = chasePosition - NPC.Center;
            NPC.spriteDirection = (directionVector.X > 0f) ? 1 : -1;
            if (Host > 0)
                NPC.realLife = (int)Host;
            if (NPC.target < 0 || NPC.target == byte.MaxValue || Main.player[NPC.target].dead)
                NPC.TargetClosest(true);
            if (Main.player[NPC.target].dead && NPC.timeLeft > 300)
                NPC.timeLeft = 300;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)Host].type != ModContent.NPCType<Gigapora>())
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0.0f, 0.0f, 0, 0, 0);
                }
            }

            if (SegmentType >= 1 && SegmentType <= 6)
            {
                NPC.width = 120;
                NPC.height = 92;
            }
            NPC host = Main.npc[(int)Host];
            Point ground = NPC.Center.ToTileCoordinates();
            if (SegmentType == 7)
            {
                NPC.width = 124;
                NPC.height = 124;
                if (host.ai[0] == 5 && host.ai[2] >= 3)
                {
                    if (Main.rand.NextBool(2) && !Framing.GetTileSafely(ground.X, ground.Y).HasTile)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<Gigapora_Rubble>(), NPC.damage, RedeHelper.PolarVector(Main.rand.Next(7, 31), NPC.rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-1, 1)));
                    }
                    Vector2 gunPos = NPC.Center + RedeHelper.PolarVector(-52 * NPC.spriteDirection, NPC.rotation) + RedeHelper.PolarVector(36, NPC.rotation + MathHelper.PiOver2);
                    for (int i = 0; i < 3; i++)
                    {
                        int d = Dust.NewDust(gunPos, 40, 104, DustID.Smoke, 0, 0, Scale: 2);
                        Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(8, 14), NPC.rotation + MathHelper.PiOver2);
                        Main.dust[d].noGravity = true;
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        int dust = Dust.NewDust(gunPos, 40, 104, ModContent.DustType<GlowDust>(), Scale: 0.5f);
                        Main.dust[dust].noGravity = true;
                        Color dustColor = new(253, 221, 3) { A = 0 };
                        Main.dust[dust].color = dustColor;
                        int dust2 = Dust.NewDust(gunPos, 40, 104, ModContent.DustType<GlowDust>(), Scale: 0.4f);
                        Main.dust[dust2].noGravity = true;
                        Color dustColor2 = new(253, 62, 3) { A = 0 };
                        Main.dust[dust2].color = dustColor2;
                    }
                }
            }
            if (SegmentType <= 0)
            {
                if (!frameLag)
                {
                    if (SegmentType >= -5)
                        NPC.frame.Y += (int)-SegmentType * 98;
                    else
                        NPC.frame.Y += (int)(-SegmentType - 6) * 98;
                    frameLag = true;
                }
                if (FrameState < 1)
                {
                    if (Main.rand.NextBool(300) && !Framing.GetTileSafely(ground.X, ground.Y).HasTile && host.ai[0] != 5 && host.ai[0] != 6)
                    {
                        FrameState = 1;
                        NPC.netUpdate = true;
                    }
                    if (host.ai[0] == 5 && host.ai[2] == 2 && SegmentType != -1 && SegmentType != -3 && SegmentType != -5 && SegmentType != -7 && SegmentType != -9 && SegmentType != -11)
                    {
                        FrameState = 1;
                        NPC.netUpdate = true;
                    }
                    if (host.ai[0] == 7 && host.ai[2] == 3)
                    {
                        FrameState = 1;
                        NPC.netUpdate = true;
                    }
                }
                else if (FrameState == 1)
                {
                    Vector2 gunPos1 = NPC.Center + RedeHelper.PolarVector(36, NPC.rotation) + RedeHelper.PolarVector(18, NPC.rotation + MathHelper.PiOver2);
                    Vector2 gunPos2 = NPC.Center + RedeHelper.PolarVector(-36, NPC.rotation) + RedeHelper.PolarVector(18, NPC.rotation + MathHelper.PiOver2);
                    if (host.ai[0] == 7 && host.ai[2] >= 3)
                    {
                        if (ShootTimer++ >= 30 && ShootTimer % 10 == 0)
                        {
                            if (Main.rand.NextBool(10))
                            {
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                                Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 2;
                                if (Main.rand.NextBool(2))
                                    NPC.Shoot(gunPos1, ModContent.ProjectileType<Gigapora_CrossBomb>(), NPC.damage, RedeHelper.PolarVector(Main.rand.Next(8, 29), NPC.rotation), SoundID.Item61);
                                else
                                    NPC.Shoot(gunPos2, ModContent.ProjectileType<Gigapora_CrossBomb>(), NPC.damage, RedeHelper.PolarVector(-Main.rand.Next(8, 29), NPC.rotation), SoundID.Item61);
                                for (int i = 0; i < 10; i++)
                                {
                                    int d = Dust.NewDust(gunPos1, 8, 20, DustID.Smoke, 0, 0, Scale: 2);
                                    Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                    Main.dust[d].noGravity = true;
                                    d = Dust.NewDust(gunPos2, 8, 20, DustID.Smoke, 0, 0, Scale: 2);
                                    Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                    Main.dust[d].noGravity = true;
                                }
                                for (int i = 0; i < 20; i++)
                                {
                                    int d = Dust.NewDust(gunPos1, 8, 20, DustID.Wraith, 0, 0, Scale: 2);
                                    Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(-8, -6), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                    Main.dust[d].noGravity = true;
                                    d = Dust.NewDust(gunPos2, 8, 20, DustID.Wraith, 0, 0, Scale: 2);
                                    Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(-8, -6), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                    Main.dust[d].noGravity = true;
                                }
                            }
                        }
                        if (ShootTimer >= 360)
                        {
                            ShootTimer = 0;
                            FrameState = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (host.ai[0] == 5 && host.ai[2] >= 2)
                    {
                        if (ShootTimer++ == 1)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<Gigapora_FlameTele>(), 0, Vector2.Zero, CustomSounds.ShieldActivate with { Pitch = -0.2f }, NPC.whoAmI, 1);
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<Gigapora_FlameTele>(), 0, Vector2.Zero, CustomSounds.ShieldActivate with { Pitch = -0.2f }, NPC.whoAmI, -1);
                        }
                        if (ShootTimer == 120)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(CustomSounds.GigaFlame with { Volume = 1.5f }, host.position);
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 4;
                            NPC.Shoot(gunPos1, ModContent.ProjectileType<Gigapora_Flame>(), NPC.damage, Vector2.Zero, NPC.whoAmI, 1);
                            NPC.Shoot(gunPos2, ModContent.ProjectileType<Gigapora_Flame>(), NPC.damage, Vector2.Zero, NPC.whoAmI, -1);
                            for (int i = 0; i < 10; i++)
                            {
                                int d = Dust.NewDust(gunPos1, 8, 20, DustID.Smoke, 0, 0, Scale: 3);
                                Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[d].velocity *= 4.5f;
                                Main.dust[d].noGravity = true;
                                d = Dust.NewDust(gunPos2, 8, 20, DustID.Smoke, 0, 0, Scale: 3);
                                Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[d].velocity *= 4.5f;
                                Main.dust[d].noGravity = true;
                            }
                            for (int i = 0; i < 20; i++)
                            {
                                int d = Dust.NewDust(gunPos1, 8, 20, DustID.Wraith, 0, 0, Scale: 3);
                                Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(-8, -6), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[d].velocity *= 5f;
                                Main.dust[d].noGravity = true;
                                d = Dust.NewDust(gunPos2, 8, 20, DustID.Wraith, 0, 0, Scale: 3);
                                Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(-8, -6), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[d].velocity *= 5f;
                                Main.dust[d].noGravity = true;
                            }
                        }
                        if (ShootTimer >= 120 && Main.rand.NextBool(50))
                        {
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 3;
                            if (Main.rand.NextBool(2))
                            {
                                NPC.Shoot(gunPos1, ModContent.ProjectileType<Gigapora_Fireball>(), NPC.damage, RedeHelper.PolarVector(Main.rand.NextFloat(24, 30), NPC.rotation), SoundID.DD2_BetsyFireballShot, NPC.whoAmI);
                                for (int i = 0; i < 10; i++)
                                {
                                    int d = Dust.NewDust(gunPos1, 8, 20, DustID.Wraith, 0, 0, Scale: 3);
                                    Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(-8, -6), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                    Main.dust[d].velocity *= 5f;
                                    Main.dust[d].noGravity = true;
                                }
                            }
                            else
                            {
                                NPC.Shoot(gunPos2, ModContent.ProjectileType<Gigapora_Fireball>(), NPC.damage, RedeHelper.PolarVector(Main.rand.NextFloat(24, 30), NPC.rotation + MathHelper.Pi), SoundID.DD2_BetsyFireballShot, NPC.whoAmI);
                                for (int i = 0; i < 10; i++)
                                {
                                    int d = Dust.NewDust(gunPos2, 8, 20, DustID.Wraith, 0, 0, Scale: 3);
                                    Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(-8, -6), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                    Main.dust[d].velocity *= 5f;
                                    Main.dust[d].noGravity = true;
                                }
                            }
                        }
                        if (ShootTimer >= 360)
                        {
                            ShootTimer = 0;
                            FrameState = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (Framing.GetTileSafely(ground.X, ground.Y).HasTile || (host.ai[0] == 5 && host.ai[2] <= 1) || (host.ai[0] == 7 && host.ai[2] <= 2))
                        {
                            ShootTimer = 0;
                            FrameState = 0;
                            NPC.netUpdate = true;
                        }
                        if (ShootTimer++ == 1)
                        {
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<Gigapora_BoltTele>(), 0, Vector2.Zero, CustomSounds.ShieldActivate, NPC.whoAmI, 1);
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<Gigapora_BoltTele>(), 0, Vector2.Zero, CustomSounds.ShieldActivate, NPC.whoAmI, -1);
                        }
                        if (ShootTimer == 60)
                        {
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = NPC.Center;
                            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 2;
                            NPC.Shoot(gunPos1, ModContent.ProjectileType<ShieldCore_Bolt>(), NPC.damage, RedeHelper.PolarVector(20, NPC.rotation), SoundID.Item62);
                            NPC.Shoot(gunPos2, ModContent.ProjectileType<ShieldCore_Bolt>(), NPC.damage, RedeHelper.PolarVector(-20, NPC.rotation), SoundID.Item62);
                            for (int i = 0; i < 10; i++)
                            {
                                int d = Dust.NewDust(gunPos1, 8, 20, DustID.Smoke, 0, 0, Scale: 3);
                                Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[d].velocity *= 4.5f;
                                Main.dust[d].noGravity = true;
                                d = Dust.NewDust(gunPos2, 8, 20, DustID.Smoke, 0, 0, Scale: 3);
                                Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(6, 8), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[d].velocity *= 4.5f;
                                Main.dust[d].noGravity = true;
                            }
                            for (int i = 0; i < 20; i++)
                            {
                                int d = Dust.NewDust(gunPos1, 8, 20, DustID.Wraith, 0, 0, Scale: 3);
                                Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(-8, -6), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[d].velocity *= 5f;
                                Main.dust[d].noGravity = true;
                                d = Dust.NewDust(gunPos2, 8, 20, DustID.Wraith, 0, 0, Scale: 3);
                                Main.dust[d].velocity = RedeHelper.PolarVector(Main.rand.NextFloat(-8, -6), NPC.rotation + Main.rand.NextFloat(-0.2f, 0.2f));
                                Main.dust[d].velocity *= 5f;
                                Main.dust[d].noGravity = true;
                            }
                        }
                        if (ShootTimer >= 80)
                        {
                            ShootTimer = 0;
                            FrameState = 0;
                            NPC.netUpdate = true;
                        }
                    }
                }
            }
            if (NPC.ai[1] < (double)Main.npc.Length)
            {
                float dirX = Main.npc[(int)NPC.ai[1]].Center.X - NPC.Center.X;
                float dirY = Main.npc[(int)NPC.ai[1]].Center.Y - NPC.Center.Y;
                NPC.rotation = (float)Math.Atan2(dirY, dirX) + 1.57f;
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                float dist = (length - NPC.width / (SegmentType >= 1 && SegmentType <= 6 ? 2f : 1.6f)) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                if (dirX < 0f)
                    NPC.spriteDirection = 1;
                else
                    NPC.spriteDirection = -1;

                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + posX;
                NPC.position.Y = NPC.position.Y + posY;
            }

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);
            NPC.netUpdate = true;
            return false;
        }
        private int CoreFrame;
        private int TailFrame;
        public override void FindFrame(int frameHeight)
        {
            if (SegmentType >= 1 && SegmentType <= 6)
            {
                if (FrameState == 1)
                    CoreFrame = 1;
                else if (FrameState == 2)
                    CoreFrame = 2;
            }
            if (SegmentType == 7 && FrameState == 2)
            {
                TailFrame = 2;
                return;
            }
            if (SegmentType <= 0 && FrameState == 2)
                NPC.frame.Y = 15 * frameHeight;
            else
            {
                if (FrameState == 1)
                {
                    if (NPC.frame.Y < 7 * frameHeight)
                        NPC.frame.Y = 7 * frameHeight;
                    if (NPC.frameCounter % 5 == 0)
                    {
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 14 * frameHeight)
                            NPC.frame.Y = 14 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.frameCounter % 5 == 0)
                    {
                        if (NPC.frame.Y > 7 * frameHeight)
                            NPC.frame.Y -= frameHeight;
                        else
                            NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y == 7 * frameHeight)
                            NPC.frame.Y = 0;
                    }
                }
            }
            if (NPC.frameCounter++ % 5 == 0)
            {
                TailFrame++;
                if (TailFrame > 1)
                    TailFrame = 0;
            }
        }
        public override bool CheckActive() => false;
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (NPC.immortal)
            {
                NPC.HitSound = CustomSounds.BallFire with { Volume = .5f };
                modifiers.SetMaxDamage(1);
                modifiers.DisableCrit();
                modifiers.HideCombatText();
                CombatText.NewText(NPC.getRect(), Color.Orange, 0, true, true);
                NPC.life++;
                return;
            }
            else
                NPC.HitSound = SoundID.NPCHit4;

            int ai3 = (int)Host;
            if (ai3 > -1 && ai3 < Main.maxNPCs && Main.npc[ai3].active && Main.npc[ai3].type == ModContent.NPCType<Gigapora>())
            {
                if (Main.npc[ai3].immune[Main.myPlayer] > 0)
                {
                    modifiers.SetMaxDamage(1);
                    modifiers.DisableCrit();
                    return;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float thrusterScaleX = MathHelper.Lerp(1.5f, 0.5f, Main.npc[(int)Host].velocity.Length() / 20);
            thrusterScaleX = MathHelper.Clamp(thrusterScaleX, 0.5f, 1.5f);
            float thrusterScaleY = MathHelper.Clamp(Main.npc[(int)Host].velocity.Length() / 10, 0.3f, 2f);
            Effect ShieldEffect = ModContent.Request<Effect>("Redemption/Effects/Shield", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D HexagonTexture = ModContent.Request<Texture2D>("Redemption/Textures/Hexagons", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 v = RedeHelper.Spread(4);
            Vector2 pos = NPC.Center;
            NPC host = Main.npc[(int)Host];
            if (host.ai[0] == 4)
                pos = NPC.Center + v;

            ShieldEffect.Parameters["offset"].SetValue(Vector2.Zero);
            ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
            ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
            ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, Main.rand.NextFloat(50f, 101f) / 100f * shieldAlpha).ToVector4());
            ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, shieldAlpha).ToVector4());
            switch (SegmentType)
            {
                case float s when s <= 0:
                    spriteBatch.End();
                    ShieldEffect.Parameters["sinMult"].SetValue(30f / 7f);
                    ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(texture.Width / 2f / (HexagonTexture.Width), texture.Height / 16 / (HexagonTexture.Height)));
                    ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (texture.Width / 2), 1f / (texture.Height / 2)));
                    ShieldEffect.Parameters["frameAmount"].SetValue(16f);
                    spriteBatch.BeginDefault(true);
                    ShieldEffect.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(texture, pos - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
                    break;
                case float s when s >= 1 && s <= 6:
                    int height = core.Value.Height / 3;
                    int y = height * CoreFrame;
                    Vector2 coreOrigin = new(core.Value.Width / 2f, height / 2f);

                    spriteBatch.End();
                    spriteBatch.BeginAdditive();

                    Vector2 thrusterOrigin = new(thrusterBlue.Value.Width / 2f, thrusterBlue.Value.Height / 2f - 20);
                    for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                    {
                        Vector2 oldPos = NPC.oldPos[i];
                        spriteBatch.Draw(FrameState == 2 ? thrusterOrange.Value : thrusterBlue.Value, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(52, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * 0.5f * MathHelper.Clamp(Main.npc[(int)Host].velocity.Length() / 20, 0, 1), oldrot[i], thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                        spriteBatch.Draw(FrameState == 2 ? thrusterOrange.Value : thrusterBlue.Value, oldPos + NPC.Size / 2f + RedeHelper.PolarVector(-52, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * 0.5f * MathHelper.Clamp(Main.npc[(int)Host].velocity.Length() / 20, 0, 1), oldrot[i], thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                    }
                    spriteBatch.Draw(FrameState == 2 ? thrusterOrange.Value : thrusterBlue.Value, pos + RedeHelper.PolarVector(52, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(Main.npc[(int)Host].velocity.Length() / 20, 0, 1), NPC.rotation, thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);
                    spriteBatch.Draw(FrameState == 2 ? thrusterOrange.Value : thrusterBlue.Value, pos + RedeHelper.PolarVector(-52, NPC.rotation) + RedeHelper.PolarVector(35, NPC.rotation + MathHelper.PiOver2) - screenPos, null, Color.White * MathHelper.Clamp(Main.npc[(int)Host].velocity.Length() / 20, 0, 1), NPC.rotation, thrusterOrigin, new Vector2(thrusterScaleX, thrusterScaleY), effects, 0);

                    spriteBatch.End();
                    ShieldEffect.Parameters["sinMult"].SetValue(30f / 6f);
                    ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(core.Value.Width / 2f / (HexagonTexture.Width), height / 2f / HexagonTexture.Height));
                    ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (core.Value.Width / 2), 1f / (core.Value.Height / 2)));
                    ShieldEffect.Parameters["frameAmount"].SetValue(3f);
                    spriteBatch.BeginDefault(true);
                    ShieldEffect.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(core.Value, pos - screenPos, new Rectangle?(new Rectangle(0, y, core.Value.Width, height)), drawColor, NPC.rotation, coreOrigin, NPC.scale, effects, 0);

                    spriteBatch.End();
                    spriteBatch.BeginDefault();
                    spriteBatch.Draw(coreGlow.Value, pos - screenPos, new Rectangle?(new Rectangle(0, y, core.Value.Width, height)), Color.White, NPC.rotation, coreOrigin, NPC.scale, effects, 0);
                    break;
                case 7:
                    int height2 = tail.Value.Height / 3;
                    int y2 = height2 * TailFrame;
                    Vector2 tailOrigin = new(tail.Value.Width / 2f, height2 / 2f);
                    spriteBatch.End();
                    ShieldEffect.Parameters["sinMult"].SetValue(30f / 4f);
                    ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(tail.Value.Width / 2f / (HexagonTexture.Width), height2 / HexagonTexture.Height));
                    ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (tail.Value.Width / 2), 1f / (tail.Value.Height / 2)));
                    ShieldEffect.Parameters["frameAmount"].SetValue(3f);
                    spriteBatch.BeginDefault(true);
                    ShieldEffect.CurrentTechnique.Passes[0].Apply();
                    spriteBatch.Draw(tail.Value, pos - screenPos, new Rectangle?(new Rectangle(0, y2, tail.Value.Width, height2)), drawColor, NPC.rotation, tailOrigin, NPC.scale, effects, 0);
                    break;
            }
            return false;
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            int ai3 = (int)Host;
            if (ai3 > -1 && ai3 < Main.maxNPCs && Main.npc[ai3].active && Main.npc[ai3].type == ModContent.NPCType<Gigapora>())
            {
                if (Main.npc[ai3].immune[Main.myPlayer] == 0)
                    Main.npc[ai3].immune[Main.myPlayer] = NPC.immune[Main.myPlayer];
            }
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            int ai3 = (int)Host;
            if (ai3 > -1 && ai3 < Main.maxNPCs && Main.npc[ai3].active && Main.npc[ai3].type == ModContent.NPCType<Gigapora>())
            {
                if (Main.npc[ai3].immune[Main.myPlayer] == 0)
                    Main.npc[ai3].immune[Main.myPlayer] = NPC.immune[Main.myPlayer];
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
    }
}
