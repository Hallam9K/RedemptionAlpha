using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class IrradiatedHardenedSandTile : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedSandstoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedSandTile>()] = true;
            Main.tileMerge[Type][TileID.HardenedSand] = true;
            Main.tileMerge[TileID.HardenedSand][Type] = true;
            Main.tileMerge[Type][TileID.CorruptHardenedSand] = true;
            Main.tileMerge[TileID.CorruptHardenedSand][Type] = true;
            Main.tileMerge[Type][TileID.CrimsonHardenedSand] = true;
            Main.tileMerge[TileID.CrimsonHardenedSand][Type] = true;
            Main.tileMerge[Type][TileID.HallowHardenedSand] = true;
            Main.tileMerge[TileID.HallowHardenedSand][Type] = true;
            TileID.Sets.Conversion.HardenedSand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.isDesertBiomeSand[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(149, 133, 121));
            MineResist = 1.5f;
            DustType = DustID.Ash;
            ItemDrop = ModContent.ItemType<IrradiatedHardenedSand>();
        }
    }
}

