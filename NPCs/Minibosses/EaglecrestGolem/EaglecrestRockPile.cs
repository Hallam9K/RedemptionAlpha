using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.NPC;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Minibosses.EaglecrestGolem
{
    public class EaglecrestRockPile : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<EaglecrestGolem>();

        public ref float AITimer => ref NPC.ai[1];

        public ref float TimerRand => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            BuffNPC.NPCTypeImmunity(Type, BuffNPC.NPCDebuffImmuneType.Inorganic);

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            ElementID.NPCEarth[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 120;
            NPC.damage = 26;
            NPC.defense = 15;
            NPC.knockBackResist = 0.1f;
            NPC.aiStyle = -1;
            NPC.width = 42;
            NPC.height = 54;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath43;
            NPC.lavaImmune = true;

            NPC.GetGlobalNPC<ElementalNPC>().OverrideMultiplier[ElementID.Earth] *= .75f;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            if (TimerRand < 4)
                NPC.LookAtEntity(player);

            if (AITimer++ == 0)
            {
                NPC.ai[0] = Main.rand.Next(300, 1200);
                TimerRand = Main.rand.NextFloat(-0.4f, 0.4f);
                NPC.netUpdate = true;
            }

            if (TimerRand >= 1)
            {
                switch (TimerRand)
                {
                    default:
                        if (NPC.ai[0] == 60)
                            NPC.velocity = new Vector2(Main.rand.NextFloat(1, 3) * NPC.spriteDirection, -Main.rand.NextFloat(3, 6));
                        if (NPC.ai[0]++ <= 60)
                            NPC.velocity.X *= .96f;
                        else
                        {
                            if (BaseAI.HitTileOnSide(NPC, 3))
                            {
                                NPC.velocity = new Vector2(Main.rand.NextFloat(2, 4) * -NPC.spriteDirection, -Main.rand.NextFloat(5, 7));
                                TimerRand = 2;
                            }
                        }
                        break;
                    case 2:
                        if (BaseAI.HitTileOnSide(NPC, 3))
                            TimerRand = 3;
                        break;
                    case 3:
                        NPC.velocity = RedeHelper.GetArcVel(NPC, player.Center, 0.3f, 50, 500, maxXvel: 20);
                        TimerRand = 4;
                        break;
                    case 4:
                        if (NPC.velocity.X == 0 || NPC.velocity.Y == 0)
                            BaseAI.DamageNPC(NPC, 999, 7, NPC.spriteDirection, NPC, false, true);
                        break;
                }
                return;
            }
            if (AITimer >= NPC.ai[0] && BaseAI.HitTileOnSide(NPC, 3))
            {
                NPC.ai[0] = 0;
                TimerRand = 1;
            }

            NPC.PlatformFallCheck(ref NPC.Redemption().fallDownPlatform, 20);
            NPCHelper.HorizontallyMove(NPC, player.Center, 0.07f, 3.8f + TimerRand, 18, 16, NPC.Center.Y > player.Center.Y, player);
        }
        public override bool? CanFallThroughPlatforms() => NPC.Redemption().fallDownPlatform;
        public override void FindFrame(int frameHeight)
        {
            if (NPC.collideY || NPC.velocity.Y == 0)
            {
                NPC.rotation.SlowRotation(0, (float)Math.PI / 20);

                NPC.frameCounter += NPC.velocity.X * 0.5f;
                if (NPC.frameCounter >= 3 || NPC.frameCounter <= -3)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 7 * frameHeight)
                        NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.rotation += NPC.velocity.X * 0.07f;
                NPC.frame.Y = 0;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("Absolute BEBE")
            });
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 25; i++)
                {
                    int dustIndex2 = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone);
                    Main.dust[dustIndex2].velocity *= 4f;
                }
                for (int i = 0; i < 2; i++)
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestRockpileGore" + (i + 1)).Type, 1);
            }
            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone);
            Main.dust[dustIndex].velocity *= 1f;
        }
    }
}


