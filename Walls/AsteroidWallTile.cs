using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class AsteroidWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = ModContent.DustType<SlateDust>();
			ItemDrop = ModContent.ItemType<AsteroidWall>();
			AddMapEntry(new Color(54, 46, 49));
		}
	}
}