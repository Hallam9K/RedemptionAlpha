using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Projectiles.Hostile;
using Redemption.Buffs.Debuffs;
using Redemption.Biomes;
using Redemption.Globals;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.Audio;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class HiveGrowth : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<PZ>();

        public ref float AITimer => ref NPC.ai[0];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hive Growth");
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 10000;
            NPC.damage = 100;
            NPC.knockBackResist = 0.1f;
            NPC.width = 58;
            NPC.height = 58;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.alpha = 225;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 35; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), Scale: 3);
                    Main.dust[dustIndex].velocity *= 2f;
                }
            }
        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return NPC.GetBestiaryEntryColor();
            }
            return null;
        }

        public override void OnKill()
        {
            Item.NewItem(NPC.getRect(), ItemID.Heart);
            if (Main.rand.NextBool(2))
                Item.NewItem(NPC.getRect(), ItemID.Heart);
        }

        public override void AI()
        {
            DespawnHandler();

            NPC.Move(Vector2.Zero, 7, 40, true);

            if (NPC.alpha > 0)
                NPC.alpha -= 20;

            if (AITimer++ >= 270)
            {
                NPC.velocity *= 0;
                if (AITimer == 315)
                {
                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<EyeFlash_Proj>(), NPC.damage, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Zap1");
                    NPC.Shoot(NPC.Center, ModContent.ProjectileType<EyeFlash_ProjH>(), NPC.damage, Vector2.Zero, true, SoundID.Item1, "Sounds/Custom/Zap1");
                }
                if (AITimer > 360)
                {
                    AITimer = 140;
                    NPC.netUpdate = true;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 2 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new FlavorTextBestiaryInfoElement("Big BEBE")
            });
        }
        private void DespawnHandler()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
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