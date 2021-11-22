using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace Redemption.NPCs.Lab
{
    public class CraneOperator : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) // TODO: Make crane operator on the bestiary when MACE is defeated
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 82;
            NPC.height = 56;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.value = 0f;
            NPC.knockBackResist = 0;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter >= 6)
            {
                NPC.frameCounter = 0;
                if (NPC.ai[0] == 1)
                {
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 3 * frameHeight)
                    {
                        NPC.ai[0] = 0;
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                else if (Main.rand.NextBool(20))
                    NPC.ai[0] = 1;
            }
        }

        public override bool CanChat() => true;
        public override string GetChat()
        {
            return "The janitor arrived after me... Guess he's never late for the job.";
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
    }
}