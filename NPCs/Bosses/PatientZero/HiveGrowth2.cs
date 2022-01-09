using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Buffs.Debuffs;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class HiveGrowth2 : ModNPC
    {
        public static int BodyType() => ModContent.NPCType<PZ>();

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
            NPC.lifeMax = 8000;
            NPC.damage = 60;
            NPC.defense = 0;
            NPC.knockBackResist = 0.1f;
            NPC.width = 48;
            NPC.height = 48;
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
        public override void AI()
        {
            DespawnHandler();

            NPC.Move(Vector2.Zero, 10, 50, true);

            if (NPC.alpha > 0)
                NPC.alpha -= 20;
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