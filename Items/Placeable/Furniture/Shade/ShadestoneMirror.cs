using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneMirror : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneMirrorTile2>(), 0);
            Item.width = 18;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 700;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 10)
                .AddIngredient(ItemID.Glass, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}