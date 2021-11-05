using Microsoft.Xna.Framework;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using Redemption.Tiles.Trees;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class DeadGrassTileCrimson : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			SetModTree(new DeadTree());
            Main.tileMerge[Type][ModContent.TileType<DeadGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<DeadGrassTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<DeadGrassTileCorruption>()] = true;
            Main.tileMerge[ModContent.TileType<DeadGrassTileCorruption>()][Type] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[Type][TileID.Grass] = true;
            Main.tileMerge[TileID.Grass][Type] = true;
            TileID.Sets.Conversion.Grass[Type] = true;
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = TileID.Dirt;
            Main.tileMergeDirt[Type] = false;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;
			AddMapEntry(new Color(106, 93, 102));
            MinPick = 10;
            MineResist = 0.1f;
            ItemDrop = ItemID.DirtBlock;
		}
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                fail = true;
                Framing.GetTileSafely(i, j).type = TileID.Dirt;
            }
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(50) && Main.tile[i, j - 1].LiquidAmount == 0)
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<DeadGrass>(), true, Main.rand.Next(5));
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<DeadGrass>(), Main.rand.Next(5), 0, -1, -1);
            }
            WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), TileID.Dirt, ModContent.TileType<DeadGrassTileCrimson>(), false, 0);

            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(100))
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
        public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return ModContent.TileType<DeadSapling>();
		}
	}
}

