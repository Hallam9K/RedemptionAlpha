using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Natural;
using Redemption.Tiles.Trees;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedSnowTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.Conversion.Ice[Type] = true;
            TileID.Sets.Ices[Type] = true;
            TileID.Sets.IcesSnow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedIceTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedIceTile>()] = true;
            Main.tileMerge[TileID.SnowBlock][Type] = true;
            Main.tileMerge[Type][TileID.SnowBlock] = true;
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[Type][TileID.IceBlock] = true;
            Main.tileMerge[TileID.IceBrick][Type] = true;
            Main.tileMerge[Type][TileID.IceBrick] = true;
            Main.tileMerge[TileID.CorruptIce][Type] = true;
            Main.tileMerge[Type][TileID.CorruptIce] = true;
            Main.tileMerge[TileID.FleshIce][Type] = true;
            Main.tileMerge[Type][TileID.FleshIce] = true;
            Main.tileMerge[TileID.HallowedIce][Type] = true;
            Main.tileMerge[Type][TileID.HallowedIce] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(187, 241, 96));
            SoundStyle = 50;
            SoundType = SoundID.Item;
            ItemDrop = ModContent.ItemType<IrradiatedSnow>();
            SetModTree(new IrradiatedBorealTree());
        }
        public override void RandomUpdate(int i, int j)
        {
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);
            if (!tileAbove.IsActive && Main.tile[i, j].IsActive && Main.rand.NextBool(100))
            {
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), true);
                NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<XenomiteCrystalTile>(), 0, 0, -1, -1);
            }
        }
        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<IrradiatedBorealSapling>();
        }
    }
}

