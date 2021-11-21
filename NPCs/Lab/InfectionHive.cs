using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Dusts;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Terraria.DataStructures;
using Redemption.Biomes;
using Terraria.GameContent.Bestiary;
using Redemption.Buffs.NPCBuffs;

namespace Redemption.NPCs.Lab
{
    public class InfectionHive : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Bleeding,
                }
            });

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0);

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 38;
            NPC.friendly = false;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 600;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
            NPC.value = 0f;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<LabBiome>().Type };
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("")
            });
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 3f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, ModContent.DustType<SludgeDust>(), NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, Scale: 3f);
        }

        public override void OnKill()
        {
            for (int i = 0; i < Main.rand.Next(3, 13); i++)
                RedeHelper.SpawnNPC((int)NPC.position.X + Main.rand.Next(0, NPC.width), (int)NPC.position.Y + Main.rand.Next(0, NPC.height), ModContent.NPCType<OozeBlob>());
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 15)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
    }
}