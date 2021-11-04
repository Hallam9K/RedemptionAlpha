using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Tiles.Tiles
{
    public class LivingDeadWoodTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<LivingDeadLeavesTile>()] = true;
            Main.tileMerge[ModContent.TileType<LivingDeadLeavesTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<DeadGrassTile>()] = true;
            Main.tileMerge[ModContent.TileType<DeadGrassTile>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<DeadGrassTileCorruption>()] = true;
            Main.tileMerge[ModContent.TileType<DeadGrassTileCorruption>()][Type] = true;
            Main.tileMerge[Type][ModContent.TileType<DeadGrassTileCrimson>()] = true;
            Main.tileMerge[ModContent.TileType<DeadGrassTileCrimson>()][Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileBlockLight[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
			AddMapEntry(new Color(90, 90, 90));
            MineResist = 2.5f;
            ItemDrop = ModContent.ItemType<PetrifiedWood>();
		}
    }
}

