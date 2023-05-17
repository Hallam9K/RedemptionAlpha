using Redemption.Tiles.Furniture.Shade;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class AngelStatueItem_SC : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Angel of the Depths Statue (Masked)");
            Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            SacrificeTotal = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AngelStatue_SC>(), 0);
			Item.width = 44;
			Item.height = 50;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Blue;
		}
	}
}