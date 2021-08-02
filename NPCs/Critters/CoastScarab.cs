using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Redemption.Items.Critters;
using Redemption.Items.Materials.PreHM;
using Microsoft.Xna.Framework;

namespace Redemption.NPCs.Critters
{
    public class CoastScarab : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 12;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath16;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 7;
            AIType = NPCID.Bunny;
            AnimationType = NPCID.LacBeetle;
            NPC.catchItem = (short)ModContent.ItemType<CoastScarabItem>();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.SandBlock, 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CoastScarabShell>(), 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Ocean.Chance * 0.4f;
        }
        public override void AI()
        {
            if (NPC.wet && Main.rand.Next(20) == 0)
            {
                int sparkle = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height / 2, DustID.SilverCoin, NPC.velocity.X * 0f, NPC.velocity.Y * 0f, 20, default, 1f);
                Main.dust[sparkle].noGravity = true;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

				new FlavorTextBestiaryInfoElement("A species of scarab commonly found scuttering around the beach. They're rather cute and harmless creatures, but can be used to create dyes if you're a greedy monster.")
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                int gore1 = ModContent.Find<ModGore>("Redemption/CoastScarabGore1").Type;
                int gore2 = ModContent.Find<ModGore>("Redemption/CoastScarabGore2").Type;
                Gore.NewGore(NPC.position, NPC.velocity, gore1, 1f);
                Gore.NewGore(NPC.position, NPC.velocity, gore2, 1f);
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.GreenBlood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
    }
}