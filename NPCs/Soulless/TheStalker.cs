using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Soulless
{
    public class TheStalker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData { ImmuneToAllBuffsThatAreNotWhips = true });
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 74;
            NPC.lifeMax = 1000;
            NPC.damage = 100;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
        }
        public override void AI()
        {
            switch (NPC.ai[0])
            {
                case 0:
                    if (SoullessArea.soullessInts[1] > 1)
                        NPC.active = false;
                    break;
                case 1:
                    switch (NPC.ai[1])
                    {
                        case 0:
                            NPC.spriteDirection = 1;
                            Player player = Main.player[Main.myPlayer];
                            Rectangle activeZone = new(473 * 16, 1097 * 16, 17 * 16, 9 * 16);
                            if (player.Hitbox.Intersects(activeZone))
                            {
                                SoullessArea.soullessInts[2] = 1;
                                NPC.ai[1] = 1;
                            }
                            break;
                        case 1:

                            break;
                    }
                    break;
            }
        }
        public override bool CheckActive() => false;
    }
}