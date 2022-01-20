using Redemption.Buffs.Debuffs;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
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

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,
                    ModContent.BuffType<InfestedDebuff>(),
                    ModContent.BuffType<NecroticGougeDebuff>(),
                    ModContent.BuffType<ViralityDebuff>(),
                    ModContent.BuffType<DirtyWoundDebuff>()
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
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
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            NPC.LookAtEntity(player);

            if (AITimer == 0)
            {
                TimerRand = Main.rand.NextFloat(-0.4f, 0.4f);
                AITimer = 1;
            }

            bool jumpDownPlatforms = false;
            NPC.JumpDownPlatform(ref jumpDownPlatforms, 20);
            if (jumpDownPlatforms) { NPC.noTileCollide = true; }
            else { NPC.noTileCollide = false; }
            RedeHelper.HorizontallyMove(NPC, player.Center, 0.07f, 3.8f + TimerRand, 18, 16, NPC.Center.Y > player.Center.Y);
        }

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

        public override void HitEffect(int hitDirection, double damage)
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
                    Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/EaglecrestRockpileGore" + (i + 1)).Type, 1);
            }
            int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Stone);
            Main.dust[dustIndex].velocity *= 1f;
        }
    }
}


