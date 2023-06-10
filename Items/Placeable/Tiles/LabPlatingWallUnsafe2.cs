using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class LabPlatingWallUnsafe2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Unsafe Laboratory Panel Wall"); 
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<LabPlatingWall>();
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<LabPlatingWallTileUnsafe2>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<LabPlatingUnsafe2>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}