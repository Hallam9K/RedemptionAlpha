using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.NPCs.Friendly;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class BastionArea : ModSystem
    {
        public override void PreUpdateWorld()
        {
            bool active = Main.LocalPlayer.InModBiome(ModContent.GetInstance<BlazingBastionBiome>());
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Terraria.Player player = Main.player[i];
                    if (!player.active)
                        continue;

                    if (player.InModBiome<BlazingBastionBiome>())
                        active = true;
                }
            }
            if (!active || RedeGen.BastionVector.X == -1)
                return;

            Vector2 NozaPos = new((RedeGen.BastionVector.X + 210) * 16, (RedeGen.BastionVector.Y + 64) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<Noza_NPC>()) && RedeBossDowned.downedPZ)
                LabArea.SpawnNPCInWorld(NozaPos, ModContent.NPCType<Noza_NPC>());
        }
    }
}