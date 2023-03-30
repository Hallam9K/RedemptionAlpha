using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneBookcase : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneBookcaseTile>(), 0);
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 60;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 20)
                .AddIngredient(ItemID.Book, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}