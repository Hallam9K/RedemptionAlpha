using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Placeable.Tiles;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Tiles
{
    public class LabPlatingTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LabPlatingTileUnsafe>()] = true;
            Main.tileMerge[ModContent.TileType<LabPlatingTileUnsafe>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrownLabPlatingTile>()] = true;
            Main.tileMerge[ModContent.TileType<OvergrownLabPlatingTile>()][Type] = true;
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 200;
            MineResist = 5f;
            HitSound = CustomSounds.MetalHit;
            AddMapEntry(new Color(202, 210, 210));
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}