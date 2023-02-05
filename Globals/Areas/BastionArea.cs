using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.NPCs.Bosses.PatientZero;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Lab;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.MACE;
using Redemption.NPCs.Lab.Volt;
using Redemption.WorldGeneration;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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
            if (!active || RedeGen.BastionPoint.X == -1 || RedeGen.BastionPoint.Y == -1)
                return;

            Vector2 NozaPos = new((RedeGen.BastionPoint.X + 210) * 16, (RedeGen.BastionPoint.Y + 64) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<Noza_NPC>()) && RedeBossDowned.downedPZ)
                LabArea.SpawnNPCInWorld(NozaPos, ModContent.NPCType<Noza_NPC>());
        }
    }
}