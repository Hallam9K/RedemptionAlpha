using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneWorkBench : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneWorkbenchTile>(), 0);
            Item.width = 32;
            Item.height = 14;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 150;
            Item.rare = ItemRarityID.Blue; 
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}