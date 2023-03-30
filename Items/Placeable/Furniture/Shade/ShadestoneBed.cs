using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneBed : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneBedTile>(), 0);
            Item.width = 34;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 2000;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 15)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}