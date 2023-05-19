using Redemption.Rarities;
using Redemption.Tiles.Furniture.Kingdom;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Items.Placeable.Furniture.Kingdom
{
    public class CeilingBrazier : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ceiling Brazier");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<CeilingBrazierTile>(), 0);
            Item.width = 24;
            Item.height = 40;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientAlloy>(4)
                .AddIngredient(ItemID.Torch, 4)
                .AddIngredient<AncientChain>(2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
