using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Ores;
using Redemption.Tiles.Trees;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedStoneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<StarliteGemOreTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCrimstoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedEbonstoneTile>()] = true;
            ItemDrop = ModContent.ItemType<IrradiatedStone>();
            TileID.Sets.Stone[Type] = true;
            TileID.Sets.Conversion.Stone[Type] = true;
            DustType = DustID.Ash;
            MinPick = 100;
            MineResist = 2.5f;
            SoundType = SoundID.Tink;
            AddMapEntry(new Color(48, 63, 73));
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileBelow2 = Framing.GetTileSafely(i, j + 2);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tileAbove2 = Framing.GetTileSafely(i, j - 2);

            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(100))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<StarliteGemTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<StarliteGemTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.IsActive && !tileAbove2.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadRockStalagmitesTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<DeadRockStalagmitesTile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadRockStalagmites2Tile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<DeadRockStalagmites2Tile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.IsActive && !tileBelow2.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<DeadRockStalacmitesTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j + 1, ModContent.TileType<DeadRockStalacmitesTile>(), 0, 0, -1, -1);
            }
            if (!tileBelow.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(400))
            {
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<DeadRockStalacmites2Tile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j + 1, ModContent.TileType<DeadRockStalacmites2Tile>(), 0, 0, -1, -1);
            }
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(600))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>());
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<XenomiteCrystalBigTile>(), 0, 0, -1, -1);
            }
        }

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<DeadSapling>();
        }
    }
}