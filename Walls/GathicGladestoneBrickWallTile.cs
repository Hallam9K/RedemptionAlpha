using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class GathicGladestoneBrickWallTile : ModWall
	{
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = false;
			DustType = ModContent.DustType<SlateDust>();
			AddMapEntry(new Color(49, 43, 39));
		}
	}
	public class GathicGladestoneBrickWallTileSafe : ModWall
	{
		public override string Texture => "Redemption/Walls/GathicGladestoneBrickWallTile";
		public override void SetStaticDefaults()
		{
			Main.wallHouse[Type] = true;
			DustType = ModContent.DustType<SlateDust>();
			AddMapEntry(new Color(49, 43, 39));
		}
	}
}