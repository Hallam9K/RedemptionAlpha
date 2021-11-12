using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class PetrifiedWoodTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileSpelunker[Type] = false;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<IrradiatedDirtTile>()] = true;
            Main.tileMerge[ModContent.TileType<IrradiatedDirtTile>()][Type] = true;
            Main.tileMerge[Type][TileID.SpookyWood] = true;
            Main.tileMerge[TileID.SpookyWood][Type] = true;
            ItemDrop = ModContent.ItemType<PetrifiedWood>();
            DustType = DustID.Ash;
            MinPick = 0;
            MineResist = 1.5f;
            AddMapEntry(new Color(111, 100, 93));
        }
    }
}