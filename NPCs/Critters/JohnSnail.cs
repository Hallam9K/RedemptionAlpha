using Redemption.Items.Critters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Redemption.NPCs.Critters
{
    public class JohnSnail : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = ModContent.NPCType<AntiJohnSnail>();
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 22;
            NPC.height = 16;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 0;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = NPCAIStyleID.Snail;
            AIType = NPCID.Snail;
            AnimationType = NPCID.Snail;
            NPC.catchItem = (short)ModContent.ItemType<JohnSnailItem>();
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.SnailCritter.Chance * 0.1f;

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Slime, NPC.velocity.X * 0.5f,
                        NPC.velocity.Y * 0.5f);
            }

            Dust.NewDust(NPC.position + NPC.velocity, NPC.width, NPC.height, DustID.t_Slime, NPC.velocity.X * 0.5f,
                NPC.velocity.Y * 0.5f);
        }
    }
}