using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Redemption.Biomes;

namespace Redemption.NPCs.Soulless
{
    public class SpookyEyes : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 130;
            NPC.height = 34;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.chaseable = false;
            NPC.npcSlots = 0;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (NPC.DistanceSQ(player.Center) < 700 * 700)
                NPC.ai[1] = 2;
        }
        public override void FindFrame(int frameHeight)
        {
            switch (NPC.ai[1])
            {
                case 0:
                    if (++NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 4 * frameHeight)
                        {
                            NPC.frame.Y = 4 * frameHeight;
                            if (Main.rand.NextBool(60))
                                NPC.ai[1] = 1;
                        }
                    }
                    break;
                case 1:
                    if (++NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 7 * frameHeight)
                        {
                            NPC.frame.Y = 4 * frameHeight;
                            NPC.ai[1] = 0;
                        }
                    }
                    break;
                case 2:
                    if (NPC.frame.Y < 7 * frameHeight)
                        NPC.frame.Y = 7 * frameHeight;

                    if (++NPC.frameCounter >= 5)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 11 * frameHeight)
                        {
                            NPC.alpha = 255;
                            NPC.active = false;
                        }
                    }
                    break;
            }
        }
    }
}