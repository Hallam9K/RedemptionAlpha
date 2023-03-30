using Terraria.ModLoader;
using Redemption.Walls;
using Terraria;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadestoneBrickWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("[c/ff0000:Unbreakable]");
			Item.ResearchUnlockCount = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableWall((ushort)ModContent.WallType<ShadestoneBrickWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}