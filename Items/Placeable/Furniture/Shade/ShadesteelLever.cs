using Redemption.Tiles.Furniture.Shade;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadesteelLever : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shadesteel Lever");
            Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            SacrificeTotal = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShadesteelLeverTile>(), 0);
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Blue;
		}
	}
}