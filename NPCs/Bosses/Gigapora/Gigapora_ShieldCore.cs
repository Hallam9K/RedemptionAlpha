using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Biomes;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Base;
using System;
using Terraria.Audio;
using Redemption.Dusts;
using Redemption.Globals.NPC;

namespace Redemption.NPCs.Bosses.Gigapora
{
    [AutoloadBossHead]
    public class Gigapora_ShieldCore : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<Gigapora>();
        public enum ActionState
        {
            Begin,
            Idle,
            Rapidfire,
            Dualcast,
            Zap,
            Death
        }

        public ActionState AIState
        {
            get => (ActionState)NPC.ai[1];
            set => NPC.ai[1] = (int)value;
        }

        public ref float AITimer => ref NPC.ai[2];

        public ref float TimerRand => ref NPC.localAI[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shield Core");
            Main.npcFrameCount[NPC.type] = 19;
            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 8000;
            NPC.damage = 80;
            NPC.defense = 25;
            NPC.knockBackResist = 0;
            NPC.width = 76;
            NPC.height = 44;
            NPC.value = 0;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<LidenBiomeOmega>().Type, ModContent.GetInstance<LidenBiome>().Type };
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.75f);
        }
        public override bool CheckDead()
        {
            if (AIState is ActionState.Death && godrayFade >= 1f)
                return true;

            if (!Main.dedServ && AIState != ActionState.Death)
                SoundEngine.PlaySound(CustomSounds.OODashReady with { Volume = 1.5f, Pitch = -.3f }, NPC.position);
            NPC.life = 1;
            AIState = ActionState.Death;
            AITimer = 0;
            TimerRand = 0;
            NPC.netUpdate = true;
            return false;
        }
        public override void OnKill()
        {
            if (AIState is ActionState.Death)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
                NPC seg = Main.npc[(int)NPC.ai[0]];
                if (seg.active && seg.type == ModContent.NPCType<Gigapora_BodySegment>())
                {
                    int steps = (int)NPC.Distance(seg.Center) / 8;
                    for (int i = 0; i < steps; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Vector2.Lerp(NPC.Center, seg.Center, (float)i / steps), 2, 2, ModContent.DustType<GlowDust>(), Scale: 2);
                        dust.noGravity = true;
                        Color dustColor = new(Color.OrangeRed.R, Color.OrangeRed.G, Color.OrangeRed.B) { A = 0 };
                        dust.color = dustColor;
                        dust.velocity = -seg.DirectionTo(dust.position) * 2;
                    }
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.MissileExplosion, seg.position);
                    RedeDraw.SpawnExplosion(seg.Center, Color.OrangeRed, DustID.RedTorch);

                    int drill = NPC.FindFirstNPC(ModContent.NPCType<Gigapora>());
                    if (drill != -1 && Main.npc[drill].active)
                    {
                        Main.npc[drill].life -= Main.npc[drill].lifeMax / 6;
                        if (Main.npc[drill].life <= 0)
                            Main.npc[drill].life = 1;
                        Main.npc[drill].netUpdate = true;
                    }

                    seg.ai[0] = 2;
                    Main.npc[seg.whoAmI - 1].ai[0] = 2;
                    Main.npc[seg.whoAmI + 1].ai[0] = 2;
                    if (Main.npc[seg.whoAmI].ai[2] == 6)
                        Main.npc[seg.whoAmI + 2].ai[0] = 2;
                    if (Main.npc[seg.whoAmI].ai[2] == 1)
                    {
                        if (drill != -1 && Main.npc[drill].active)
                            Main.npc[drill].ai[3] = 1;
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && AIState is ActionState.Death && godrayFade >= 1f)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/ShieldCoreGore" + (i + 1)).Type);
                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.LifeDrain, Scale: 2);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                for (int i = 0; i < 6; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Electric);
                    Main.dust[dustIndex].velocity *= 2f;
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override bool CheckActive() => false;
        private float godrayFade;
        private float godraySize;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC seg = Main.npc[(int)NPC.ai[0]];
            if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>())
                NPC.active = false;
            if (DespawnHandler())
                return;

            bool another = NPC.CountNPCS(ModContent.NPCType<Gigapora_ShieldCore>()) > 1;

            switch (AIState)
            {
                case ActionState.Begin:
                    NPC.velocity *= 0.99f;
                    if (AITimer++ >= 85)
                    {
                        AITimer = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Idle:
                    if (NPC.DistanceSQ(player.Center) < 400 * 400)
                        NPC.Move(Vector2.Zero, NPC.DistanceSQ(player.Center) < 100 * 100 ? 18 : 6, 60, true, true);
                    else if (NPC.DistanceSQ(player.Center) >= 500 * 500)
                        NPC.Move(Vector2.Zero, NPC.DistanceSQ(player.Center) > 1100 * 1100 ? 18 : 10, 60, true);
                    else
                        NPC.velocity *= 0.98f;

                    if (AITimer++ % (another ? 120 : 90) == 0)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<ShieldCore_Bolt>(), NPC.damage, NPC.DirectionTo(player.Center) * 8, CustomSounds.Laser1);
                    }
                    if (AITimer >= (another ? 340 : 220) && NPC.DistanceSQ(player.Center) <= 600 * 600)
                    {
                        AITimer = 0;
                        AIState = (ActionState)Main.rand.Next(2, 5);
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Rapidfire:
                    AIState = (ActionState)Main.rand.Next(2, 5);
                    break;
                case ActionState.Dualcast:
                    NPC.velocity *= 0.98f;
                    if (AITimer++ == 30)
                    {
                        NPC.velocity = player.Center.DirectionTo(NPC.Center) * 3;
                        for (int i = -1; i <= 1; i += 2)
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<ShieldCore_DualcastBall>(), (int)(NPC.damage * 1.15f), RedeHelper.PolarVector(14, (player.Center - NPC.Center).ToRotation() - MathHelper.ToRadians(80 * i)), CustomSounds.Zap2);
                    }
                    if (AITimer >= 60)
                    {
                        AITimer = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Zap:
                    NPC.Move(new Vector2(Main.rand.Next(-100, 100), -400), 18, 50, true);
                    if (AITimer++ >= 20 && AITimer % (another ? 8 : 5) == 0 && AITimer <= 80)
                    {
                        NPC.Shoot(player.Center + player.velocity + RedeHelper.Spread(300), ModContent.ProjectileType<ShieldCore_Zap>(), NPC.damage, Vector2.Zero, CustomSounds.ElectricSlash, NPC.whoAmI);
                    }
                    if (AITimer >= 140)
                    {
                        AITimer = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Death:
                    NPC.velocity *= .8f;
                    NPC.velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                    godrayFade += 0.01f;
                    godraySize += 0.01f;
                    if (godrayFade >= 1f)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(CustomSounds.MissileExplosion with { Pitch = .1f }, NPC.position);
                        RedeDraw.SpawnExplosion(NPC.Center, Color.OrangeRed, DustID.RedTorch);
                        NPC.dontTakeDamage = false;
                        player.ApplyDamageToNPC(NPC, 9999, 0, 0, false);
                        if (Main.netMode == NetmodeID.Server && NPC.whoAmI < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                    }
                    break;
            }
            OverlapCheck();
            if (NPC.ai[3] > 0)
            {
                NPC.ai[3]++;
                if (NPC.DistanceSQ(player.Center) >= 500 * 500)
                    NPC.velocity *= 0.9f;
                if (NPC.ai[3] >= 180)
                    NPC.ai[3] = 0;
            }
        }
        private void OverlapCheck()
        {
            float overlapVelocity = 0.04f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];

                if (i != NPC.whoAmI && other.active && Math.Abs(NPC.position.X - other.position.X) + Math.Abs(NPC.position.Y - other.position.Y) < NPC.width)
                {
                    if (NPC.position.X < other.position.X)
                        NPC.velocity.X -= overlapVelocity;
                    else
                        NPC.velocity.X += overlapVelocity;

                    if (NPC.position.Y < other.position.Y)
                        NPC.velocity.Y -= overlapVelocity;
                    else
                        NPC.velocity.Y += overlapVelocity;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.rotation = NPC.velocity.X * 0.07f;
            if (++NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 18 * frameHeight)
                    NPC.frame.Y = 10 * frameHeight;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Absolute BEBE")
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowMask = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Texture2D glowRadius = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.BeginAdditive();

                float glowOpacity = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.3f, 0.5f, 0.3f);
                float glowSize = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.5f, 1f, 1.5f);
                float glowSize2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 1.5f, 1f);
                Vector2 glowOrigin = new(glowRadius.Width / 2f, glowRadius.Height / 2f);
                spriteBatch.Draw(glowRadius, NPC.Center - screenPos, null, Color.Red * glowOpacity, 0, glowOrigin, glowSize, effects, 0);
                spriteBatch.Draw(glowRadius, NPC.Center - screenPos, null, Color.Red * glowOpacity * 0.7f, 0, glowOrigin, glowSize2 * 1.5f, effects, 0);

                float dist = NPC.Distance(Main.npc[(int)NPC.ai[0]].Center);
                float heatOpacity = 1500f / dist;

                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraph").Value, NPC.Center - screenPos, new Rectangle(0, 0, 64, 128), Color.Red * glowOpacity * MathHelper.Lerp(0f, 1f, heatOpacity), (Main.npc[(int)NPC.ai[0]].Center - NPC.Center).ToRotation(), new Vector2(0, 64), new Vector2(dist / 100, NPC.width / 128f), SpriteEffects.None, 0f);
                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraphCap").Value, NPC.Center - screenPos, new Rectangle(0, 0, 64, 128), Color.Red * glowOpacity * MathHelper.Lerp(0f, 1f, heatOpacity), (NPC.Center - Main.npc[(int)NPC.ai[0]].Center).ToRotation(), new Vector2(0, 64), new Vector2(NPC.width / 128f, NPC.width / 128f), SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.BeginDefault();
            }
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            if (godrayFade > 0)
            {
                float fluctuate = (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f)) * 0.1f;
                float modifiedScale = NPC.scale * (1 + fluctuate);

                Color godrayColor = Color.Lerp(new Color(255, 255, 255), Color.OrangeRed * NPC.Opacity, 0.5f);
                godrayColor.A = 0;
                RedeDraw.DrawGodrays(Main.spriteBatch, NPC.Center - Main.screenPosition, godrayColor * godrayFade, 220 * modifiedScale * godraySize, 55 * modifiedScale * godraySize, 16);
            }
            return false;
        }

        private bool DespawnHandler()
        {
            NPC seg = Main.npc[(int)NPC.ai[0]];
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead || seg.ai[0] == 2)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || seg.ai[0] == 2)
                {
                    NPC.velocity.Y = -10;
                    if (NPC.timeLeft > 10)
                        NPC.timeLeft = 10;
                    return true;
                }
            }
            return false;
        }
    }
}
