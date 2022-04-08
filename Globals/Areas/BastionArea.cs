using Microsoft.Xna.Framework;
using Redemption.NPCs.Friendly;
using Redemption.WorldGeneration;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class BastionArea : ModSystem
    {
        public static bool Active;
        public override void PreUpdateEntities()
        {
            Active = false;
        }
        public override void PreUpdateWorld()
        {
            if (!Active || RedeGen.BastionVector.X == -1 || RedeGen.BastionVector.Y == -1 || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 NozaPos = new((RedeGen.BastionVector.X + 210) * 16, (RedeGen.BastionVector.Y + 64) * 16);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<Noza_NPC>()) && RedeBossDowned.downedPZ)
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)NozaPos.X, (int)NozaPos.Y, ModContent.NPCType<Noza_NPC>());
        }
    }
}