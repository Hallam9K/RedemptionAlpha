using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Ores;

namespace Redemption.Tiles.Tiles
{
    public class EvergoldBrickTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientDirtTile>()] = true;
            Main.tileMerge[Type][ModContent.TileType<AncientAlloyBrickTile>()] = true;
            Main.tileMerge[ModContent.TileType<AncientDirtTile>()][Type] = true;
            Main.tileMerge[ModContent.TileType<AncientAlloyBrickTile>()][Type] = true;
            DustType = DustID.GoldCoin;
            ItemDrop = ModContent.ItemType<EvergoldBrick>();
            MinPick = 500;
            MineResist = 18f;
            SoundType = SoundID.Tink;
            AddMapEntry(new Color(230, 230, 50));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}