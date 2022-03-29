using Terraria.ModLoader;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class LeadFenceBlackWallItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lead Fence (Black Background)");
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<LeadFenceBlackWall>());
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
		}
	}
}