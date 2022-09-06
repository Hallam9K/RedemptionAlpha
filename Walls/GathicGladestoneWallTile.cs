using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class GathicGladestoneWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
            Main.wallHouse[Type] = false;
            DustType = ModContent.DustType<SlateDust>();
			ItemDrop = ModContent.ItemType<GathicGladestoneWall>();
			AddMapEntry(new Color(49, 43, 39));
		}
	}
}