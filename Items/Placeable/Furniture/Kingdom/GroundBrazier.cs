using Redemption.Rarities;
using Redemption.Tiles.Furniture.Kingdom;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Items.Placeable.Furniture.Kingdom
{
    public class GroundBrazier : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ceiling Brazier");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<GroundBrazierTile>(), 0);
            Item.width = 24;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientAlloy>(6)
                .AddIngredient(ItemID.Torch, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
