using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedCrimsonGrassTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedGrassTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCorruptGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedCorruptGrassTile>()][Type] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[Type][TileID.Grass] = true;
            Main.tileMerge[TileID.Grass][Type] = true;
            Main.tileMerge[Type][TileID.CorruptGrass] = true;
            Main.tileMerge[TileID.CorruptGrass][Type] = true;
            Main.tileMerge[Type][TileID.CrimsonGrass] = true;
            Main.tileMerge[TileID.CrimsonGrass][Type] = true;
            Main.tileMerge[Type][TileID.HallowedGrass] = true;
            Main.tileMerge[TileID.HallowedGrass][Type] = true;
            TileID.Sets.Conversion.Grass[Type] = true;
            TileID.Sets.Conversion.MergesWithDirtInASpecialWay[Type] = true;
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.Crimson[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<IrradiatedDirtTile>();
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.ResetsHalfBrickPlacementAttempt[Type] = true;
            TileID.Sets.DoesntPlaceWithTileReplacement[Type] = true;
            TileID.Sets.SpreadOverground[Type] = true;
            TileID.Sets.SpreadUnderground[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			AddMapEntry(new Color(106, 93, 102));
            MinPick = 10;
            MineResist = 0.1f;
            DustType = DustID.Ash;
            ItemDrop = ModContent.ItemType<IrradiatedDirt>();
		}
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                fail = true;
                Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<IrradiatedDirtTile>();
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            Tile tile = Framing.GetTileSafely(i, j);
            if (WorldGen.genRand.NextBool(15) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
            {
                if (tile.Slope != SlopeType.SlopeUpLeft && tile.Slope != SlopeType.SlopeUpRight)
                {
                    tileBelow.TileType = (ushort)ModContent.TileType<CrimsonWastelandVine>();
                    tileBelow.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                }
            }

            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(15) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<CrimsonWastelandFoliage>(), true, Main.rand.Next(22));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<CrimsonWastelandFoliage>(), Main.rand.Next(22), 0, -1, -1);
            }
            if (Main.rand.NextBool(4))
                WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), ModContent.TileType<IrradiatedDirtTile>(), Type, false, 0);

            if (!tileAbove.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(100))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), 0, 0, -1, -1);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.04f;
            b = 0.0f;
        }
	}
}

