using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab
{
    public class SludgeBlob : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sludge Blob");
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                    BuffID.Poisoned
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 14;
            NPC.friendly = false;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.lifeMax = 100;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.value = 0f;
            NPC.knockBackResist = 0.6f;
            NPC.aiStyle = 1;
            AIType = NPCID.Crimslime;
            AnimationType = NPCID.SlimeMasked;
            NPC.lavaImmune = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement("Blob blob blob.")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 10; i++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SludgeDust>());
            }
            Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SludgeDust>());
        }
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.rand.NextBool(2) || Main.expertMode)
                target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), Main.rand.Next(60, 120));
        }
    }
}