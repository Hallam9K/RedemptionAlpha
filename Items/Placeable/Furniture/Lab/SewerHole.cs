using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class SewerHole : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sewer Hole");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SewerHoleTile>(), 0);
			Item.width = 26;
			Item.height = 28;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 100;
			Item.rare = ItemRarityID.LightPurple;
		}
    }
}