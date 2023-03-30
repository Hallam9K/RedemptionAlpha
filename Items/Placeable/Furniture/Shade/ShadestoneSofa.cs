using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneSofa : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneSofaTile>(), 0);
            Item.width = 34;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 60;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 5)
                .AddIngredient(ItemID.Silk, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}