using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class MossTube : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MossTubeTile>(), 0);
            Item.width = 38;
            Item.height = 40;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 2500;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<LabPlating>(4)
               .AddIngredient(ItemID.Glass, 14)
               .AddIngredient(ItemID.GrassSeeds, 4)
               .AddTile(TileID.WorkBenches)
               .Register();
        }
    }
}