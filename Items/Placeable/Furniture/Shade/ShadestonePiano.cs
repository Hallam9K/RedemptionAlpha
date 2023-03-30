using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestonePiano : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestonePianoTile>(), 0);
            Item.width = 34;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 60;
            Item.rare = ItemRarityID.Blue; 
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 4)
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 15)
                .AddIngredient(ItemID.Book)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}