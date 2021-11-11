using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Plants;
using Redemption.Tiles.Trees;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedGrassTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            SetModTree(new IrradiatedPurityTree());
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCrimsonGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedCrimsonGrassTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedCorruptGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedCorruptGrassTile>()][Type] = true;
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
            AddMapEntry(new Color(102, 103, 100));
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
            WorldGen.SpreadGrass(i + Main.rand.Next(-1, 1), j + Main.rand.Next(-1, 1), TileID.Dirt, ModContent.TileType<IrradiatedGrassTile>(), false, 0);

            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(100))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), 0, 0, -1, -1);
            }
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<DeadSapling>();
        }
    }
}

