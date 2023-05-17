using Microsoft.Xna.Framework;
using Redemption.Biomes;
using Redemption.NPCs.Space;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
    public class SpaceArea : ModSystem
    {
        public static Vector2 base3Vector = new(-1, -1);
        public static Vector2 base6Vector = new(-1, -1);
        public override void OnWorldLoad()
        {
        }
        public override void OnWorldUnload()
        {
        }
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
            Vector2 LiftPos2 = new((base3Vector.X + 42) * 16, (base3Vector.Y + 73) * 16);
            if (base3Vector.X != -1 && !Terraria.NPC.AnyNPCs(ModContent.NPCType<SlayerBaseLift2>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos2.X, (int)LiftPos2.Y, ModContent.NPCType<SlayerBaseLift2>(), 0, 0, 0, base3Vector.Y + 71, base3Vector.Y + 26);
            if (base3Vector.X != -1 && base6Vector.X != -1 && !Terraria.NPC.AnyNPCs(ModContent.NPCType<AndroidSitting>()))
            {
                Vector2 AndroidPos1 = new((base3Vector.X + 79) * 16 + 4, (base3Vector.Y + 42) * 16 + 14);
                Vector2 AndroidPos2 = new((base3Vector.X + 85) * 16 + 12, (base3Vector.Y + 42) * 16 + 14);
                Terraria.NPC.NewNPC(new EntitySource_WorldGen(), (int)AndroidPos1.X, (int)AndroidPos1.Y, ModContent.NPCType<AndroidSitting>(), 0, 1);
                Terraria.NPC.NewNPC(new EntitySource_WorldGen(), (int)AndroidPos2.X, (int)AndroidPos2.Y, ModContent.NPCType<AndroidSitting>(), 0, -1);
                Vector2 AndroidPos3 = new((base6Vector.X + 65) * 16 + 4, (base6Vector.Y + 90) * 16 + 14);
                Vector2 AndroidPos4 = new((base6Vector.X + 71) * 16 + 12, (base6Vector.Y + 90) * 16 + 14);
                Terraria.NPC.NewNPC(new EntitySource_WorldGen(), (int)AndroidPos3.X, (int)AndroidPos3.Y, ModContent.NPCType<AndroidSitting>(), 0, 1);
                Terraria.NPC.NewNPC(new EntitySource_WorldGen(), (int)AndroidPos4.X, (int)AndroidPos4.Y, ModContent.NPCType<AndroidSitting>(), 0, -1);
            }
            Vector2 LiftPos3 = new((base6Vector.X + 87) * 16, (base6Vector.Y + 61) * 16);
            if (base6Vector.X != -1 && !Terraria.NPC.AnyNPCs(ModContent.NPCType<SlayerBaseLift3>()))
                Terraria.NPC.NewNPC(new EntitySource_SpawnNPC(), (int)LiftPos3.X, (int)LiftPos3.Y, ModContent.NPCType<SlayerBaseLift3>(), 0, 0, 0, base6Vector.Y + 97, base6Vector.Y + 59);
        }
        public override void SaveWorldData(TagCompound tag)
        {
            tag["base3VectorX"] = base3Vector.X;
            tag["base3VectorY"] = base3Vector.Y;
            tag["base6VectorX"] = base6Vector.X;
            tag["base6VectorY"] = base6Vector.Y;
        }
        public override void LoadWorldData(TagCompound tag)
        {
            base3Vector.X = tag.GetFloat("base3VectorX");
            base3Vector.Y = tag.GetFloat("base3VectorY");
            base6Vector.X = tag.GetFloat("base6VectorX");
            base6Vector.Y = tag.GetFloat("base6VectorY");
        }
        public override void NetSend(BinaryWriter writer)
        {
            writer.WritePackedVector2(base3Vector);
            writer.WritePackedVector2(base6Vector);
        }
        public override void NetReceive(BinaryReader reader)
        {
            base3Vector = reader.ReadPackedVector2();
            base6Vector = reader.ReadPackedVector2();
        }
    }
}