using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Natural;
using Redemption.Buffs.Debuffs;
using Redemption.Tiles.Tiles;
using Redemption.Items.Materials.HM;

namespace Redemption.Tiles.Ores
{
    public class StarliteGemOreTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 600;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedStoneTile>()] = true;
            ItemDrop = ModContent.ItemType<Starlite>();
            MinPick = 180;
            MineResist = 3.5f;
            SoundType = SoundID.Tink;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Starlite");
            AddMapEntry(new Color(30, 65, 25), name);
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j + 2);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tileAbove2 = Framing.GetTileSafely(i, j - 2);

            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(12))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<StarliteGemTile>(), true, Main.rand.Next(4));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<StarliteGemTile>(), Main.rand.Next(4), 0, -1, -1);
            }
            if (!tileAbove.IsActive && !tileAbove2.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(12))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadRockStalagmitesTile>(), true, Main.rand.Next(3));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<DeadRockStalagmitesTile>(), Main.rand.Next(3), 0, -1, -1);
            }
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(12))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadRockStalagmites2Tile>(), true, Main.rand.Next(3));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<DeadRockStalagmites2Tile>(), Main.rand.Next(3), 0, -1, -1);
            }
            if (!tileBelow.IsActive && !tileBelow2.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(12))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadRockStalacmitesTile>(), true, Main.rand.Next(3));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<DeadRockStalacmitesTile>(), Main.rand.Next(3), 0, -1, -1);
            }
            if (!tileBelow.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(12))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadRockStalacmites2Tile>(), true, Main.rand.Next(3));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<DeadRockStalacmites2Tile>(), Main.rand.Next(3), 0, -1, -1);
            }
        }
    }
}