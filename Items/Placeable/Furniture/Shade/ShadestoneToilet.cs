using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneToilet : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneToiletTile>(), 0);
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = 1000;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 6)
               .AddTile(TileID.Anvils)
               .Register();
        }
    }
}