using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Biomes;
using Terraria.DataStructures;
using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.Base;

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
            Zap
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
            DisplayName.SetDefault("Shield Core");
            Main.npcFrameCount[NPC.type] = 19;
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

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 5000;
            NPC.damage = 60;
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
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.6f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }
        public override void OnKill()
        {
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
            Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
            NPC seg = Main.npc[(int)NPC.ai[0]];
            if (seg.active && seg.type == ModContent.NPCType<Gigapora_BodySegment>())
            {
                seg.ai[0] = 2;
                Main.npc[seg.whoAmI - 1].ai[0] = 2;
                Main.npc[seg.whoAmI + 1].ai[0] = 2;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
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
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CheckActive() => false;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC seg = Main.npc[(int)NPC.ai[0]];
            if (!seg.active || seg.type != ModContent.NPCType<Gigapora_BodySegment>())
                NPC.active = false;
            DespawnHandler();

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
                    if (NPC.DistanceSQ(player.Center) >= 300 * 300)
                        NPC.Move(Vector2.Zero, NPC.DistanceSQ(player.Center) > 700 * 700 ? 18 : 10, 60, true);
                    else
                        NPC.velocity *= 0.98f;

                    if (AITimer++ % (another ? 90 : 60) == 0)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<ShieldCore_Bolt>(), NPC.damage, NPC.DirectionTo(player.Center) * 8, true, SoundID.Item1, "Sounds/Custom/Laser1");
                    }
                    if (AITimer >= (another ? 220 : 180) && NPC.DistanceSQ(player.Center) <= 600 * 600)
                    {
                        AITimer = 0;
                        AIState = (ActionState)Main.rand.Next(2, 5);
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Rapidfire:
                    NPC.velocity *= 0.98f;
                    if (AITimer++ == 0)
                        NPC.velocity = player.Center.DirectionTo(NPC.Center) * 6;

                    if (AITimer++ % 3 == 0 && AITimer < 30)
                    {
                        NPC.Shoot(NPC.Center, ModContent.ProjectileType<ShieldCore_Bolt>(), NPC.damage, RedeHelper.PolarVector(6, (player.Center - NPC.Center).ToRotation() + TimerRand - MathHelper.ToRadians(another ? 50 : 25)), true, SoundID.Item1, "Sounds/Custom/Laser1");

                        TimerRand += MathHelper.ToRadians(another ? 20 : 10);
                        NPC.netUpdate = true;
                    }
                    if (AITimer >= 50)
                    {
                        AITimer = 0;
                        TimerRand = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
                case ActionState.Dualcast:
                    NPC.velocity *= 0.98f;
                    if (AITimer++ == 30)
                    {
                        NPC.velocity = player.Center.DirectionTo(NPC.Center) * 3;
                        for (int i = -1; i <= 1; i += 2)
                            NPC.Shoot(NPC.Center, ModContent.ProjectileType<ShieldCore_DualcastBall>(), (int)(NPC.damage * 1.15f), RedeHelper.PolarVector(14, (player.Center - NPC.Center).ToRotation() - MathHelper.ToRadians(80 * i)), true, SoundID.Item1, "Sounds/Custom/Zap2");
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
                        NPC.Shoot(player.Center + player.velocity + RedeHelper.Spread(300), ModContent.ProjectileType<ShieldCore_Zap>(), NPC.damage, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/ElectricSlash", NPC.whoAmI);
                    }
                    if (AITimer >= 140)
                    {
                        AITimer = 0;
                        AIState = ActionState.Idle;
                        NPC.netUpdate = true;
                    }
                    break;
            }

            if (NPC.ai[3] > 0)
            {
                NPC.ai[3]++;
                NPC.velocity *= 0.9f;
                if (NPC.ai[3] >= 180)
                    NPC.ai[3] = 0;
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
            Texture2D glowMask = ModContent.Request<Texture2D>(NPC.ModNPC.Texture + "_Glow").Value;
            Texture2D glowRadius = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!NPC.IsABestiaryIconDummy)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                float glowOpacity = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0.3f, 0.5f, 0.3f);
                float glowSize = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1.5f, 1f, 1.5f);
                float glowSize2 = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 1f, 1.5f, 1f);
                Vector2 glowOrigin = new(glowRadius.Width / 2f, glowRadius.Height / 2f);
                spriteBatch.Draw(glowRadius, NPC.Center - screenPos, null, Color.Red * glowOpacity, 0, glowOrigin, glowSize, effects, 0);
                spriteBatch.Draw(glowRadius, NPC.Center - screenPos, null, Color.Red * glowOpacity * 0.7f, 0, glowOrigin, glowSize2 * 1.5f, effects, 0);

                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraph").Value, NPC.Center - screenPos, new Rectangle(0, 0, 64, 128), Color.Red * glowOpacity, (Main.npc[(int)NPC.ai[0]].Center - NPC.Center).ToRotation(), new Vector2(0, 64), new Vector2(NPC.Distance(Main.npc[(int)NPC.ai[0]].Center) / 150, NPC.width / 128f), SpriteEffects.None, 0f);
                spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraphCap").Value, NPC.Center - screenPos, new Rectangle(0, 0, 64, 128), Color.Red * glowOpacity, (NPC.Center - Main.npc[(int)NPC.ai[0]].Center).ToRotation(), new Vector2(0, 64), new Vector2(NPC.width / 128f, NPC.width / 128f), SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glowMask, NPC.Center - screenPos, NPC.frame, RedeColor.RedPulse, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

            return false;
        }

        private void DespawnHandler()
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
                    return;
                }
            }
        }
    }
}
