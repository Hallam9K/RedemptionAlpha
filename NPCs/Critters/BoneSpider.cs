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
    public class BoneSpider : ModNPC
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
            NPC.height = 10;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath16;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 7;
            AIType = NPCID.Bunny;
            AnimationType = NPCID.LacBeetle;
            NPC.catchItem = (short)ModContent.ItemType<BoneSpiderItem>();
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.Cavern.Chance * 0.1f;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				new FlavorTextBestiaryInfoElement("An skeletal spider ready to spook you at any moment!")
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
                }
            }
            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.Bone, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f);
        }
    }
}