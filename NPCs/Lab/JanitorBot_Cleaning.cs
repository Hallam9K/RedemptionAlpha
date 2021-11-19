using Microsoft.Xna.Framework;
using Redemption.Items.Usable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Lab
{
    public class JanitorBot_Cleaning : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Janitor");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.width = 34;
            NPC.height = 44;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.npcSlots = 0;
            NPC.netAlways = true;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ >= 20)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > 3 * frameHeight)
                    NPC.frame.Y = 0;
            }
        }
        public override bool CanChat() => true;
        public override string GetChat()
        {
            return "SOMEONE needs to clean this place up before everyone else arrives...";
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
    }
}