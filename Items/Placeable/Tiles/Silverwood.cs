using Redemption.Items.Placeable.Furniture.Silverwood;
using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Placeable.Tiles
{
    public class Silverwood : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Wood;
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodTile>());
            Item.width = 24;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SilverwoodPlatform>(), 2)
                .Register();
            CreateRecipe()
                .AddIngredient<LivingSilverwoodWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
