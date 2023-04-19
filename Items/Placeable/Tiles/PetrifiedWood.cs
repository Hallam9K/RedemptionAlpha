using Redemption.Items.Placeable.Furniture.PetrifiedWood;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class PetrifiedWood : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Wood;
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodTile>(), 0);
            Item.width = 24;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PetrifiedWoodPlatform>(), 2)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PetrifiedWoodWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<PetrifiedWoodFence>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
