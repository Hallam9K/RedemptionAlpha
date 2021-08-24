using Redemption.Items.Placeable.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.PreHM
{
    public class RaggedZombie : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Velocity = 1f
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 46;
            NPC.friendly = false;
            NPC.damage = 13;
            NPC.defense = 6;
            NPC.lifeMax = 44;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 60;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 3;
            AIType = NPCID.Zombie;
            AnimationType = NPCID.Zombie;
            Banner = Item.NPCtoBanner(NPCID.Zombie);
            BannerItem = Item.BannerToItem(Banner);
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)          //this make so when the npc has 0 life(dead) he will spawn this
            {
                for (int i = 0; i < 12; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);

                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Redemption/ZomGore" + (i + 1)).Type, 1);

            }
            for (int i = 0; i < 12; i++)
                Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 50));
            npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 200));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            float baseChance = SpawnCondition.OverworldNightMonster.Chance;
            float multiplier = Main.hardMode ? 0.01f : 0.04f;

            return baseChance * multiplier;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

                new FlavorTextBestiaryInfoElement(
                    "A common creature native to Anglon, living in lush forests. They are made out of plant fibre and roots.")
            });
        }
    }
}