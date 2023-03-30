using Terraria.ModLoader;
using Redemption.Walls;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class LeadFenceBlackWallItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lead Fence (Black Background)");
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableWall((ushort)ModContent.WallType<LeadFenceBlackWall>());
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}