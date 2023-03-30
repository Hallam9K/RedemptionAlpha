using Terraria.ModLoader;
using Redemption.Walls;
using Redemption.Rarities;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadestoneWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableWall((ushort)ModContent.WallType<ShadestoneWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}