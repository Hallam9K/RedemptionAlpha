using Terraria.ModLoader;
using Redemption.Walls;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadestoneBrickWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("[c/ff0000:Unbreakable]");
			SacrificeTotal = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<ShadestoneBrickWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 9999;
			Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}