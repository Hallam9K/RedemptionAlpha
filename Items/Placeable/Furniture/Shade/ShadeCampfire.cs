using Redemption.Items.Placeable.Tiles;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Shade;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadeCampfire : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("Life regen is increased when near a campfire");
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerCampfire;
            Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShadeCampfireTile>(), 0);
			Item.width = 32;
			Item.height = 18;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(0, 0, 0, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Shadestone>(10)
				.AddIngredient<ShadeTorch>(5)
				.Register();
		}
	}
}