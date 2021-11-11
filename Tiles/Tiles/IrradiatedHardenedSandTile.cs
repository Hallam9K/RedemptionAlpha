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
            Main.tileMerge[Type][ModContent.TileType<IrradiatedSandstoneTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedSandTile>()] = true;
            TileID.Sets.Conversion.HardenedSand[Type] = true;
            TileID.Sets.isDesertBiomeSand[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            AddMapEntry(new Color(48, 63, 73));
            MineResist = 1.5f;
            DustType = ModContent.DustType<IrradiatedStoneDust>();
            ItemDrop = ModContent.ItemType<IrradiatedHardenedSand>();
        }
    }
}

