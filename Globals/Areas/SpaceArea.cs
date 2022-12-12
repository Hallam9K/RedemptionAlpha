using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.NPCs.Space;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class SpaceArea : ModSystem
    {
        public override void PreUpdateWorld()
        {
            bool active = Main.LocalPlayer.InModBiome<SpaceBiome>();
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Terraria.Player player = Main.player[i];
                    if (!player.active)
                        continue;

                    if (player.InModBiome<SpaceBiome>())
                        active = true;
                }
            }
            if (!active || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 LiftPos = new(((2400 / 2) + 76) * 16, 583 * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<SlayerBaseLift>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos.X, (int)LiftPos.Y, ModContent.NPCType<SlayerBaseLift>(), 0, 0, 0, 581, 543);
        }
    }
}