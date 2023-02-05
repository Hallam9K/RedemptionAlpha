using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class GathicStoneTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientHallBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientDirtTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicStoneBrickTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<GathicGladestoneTile>()] = true;
            ItemDrop = ModContent.ItemType<GathicStone>();
            DustType = ModContent.DustType<SlateDust>();
            MinPick = 0;
            MineResist = 1.5f;
            AddMapEntry(new Color(81, 72, 65));
        }
    }
}