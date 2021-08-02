using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Redemption.Items.Critters;
using Redemption.Items.Materials.PreHM;

namespace Redemption.NPCs.Critters
{
    public class TreeBug : ModNPC
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
            NPC.width = 20;
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
            NPC.catchItem = (short)ModContent.ItemType<TreeBugItem>();
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ProjTags.Fire.Has(projectile.type))
            {
                damage *= 500;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Wood, 1, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TreeBugShell>(), 3));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDayGrassCritter.Chance * (Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == TileID.Grass ? 1.7f : 0f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

				new FlavorTextBestiaryInfoElement("A beetle commonly found inhabiting trees. It feeds on leaves and uses its leaf-like shell for camouflage from predators. Its shell makes a good source of green dye.")
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                int gore1 = ModContent.Find<ModGore>("Redemption/TreeBugGore1").Type;
                int gore2 = ModContent.Find<ModGore>("Redemption/TreeBugGore2").Type;
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