using Microsoft.Xna.Framework;
using Redemption.NPCs.Soulless;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class SoullessArea : ModSystem
    {
        public static bool Active;
        public override void PreUpdateEntities()
        {
            Active = false;
        }
        public override void PreUpdateWorld()
        {
            if (!Active || Main.netMode == NetmodeID.MultiplayerClient)
                return;

            Vector2 LiftPos = new(608 * 16, (822 * 16) + 8);
            if (!Terraria.NPC.AnyNPCs(ModContent.NPCType<ShadestoneLift>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos.X, (int)LiftPos.Y, ModContent.NPCType<ShadestoneLift>());
        }
    }
}