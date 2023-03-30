using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneTable2 : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Long Shadestone Table");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneTable2Tile>(), 0);
            Item.width = 58;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 500;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}