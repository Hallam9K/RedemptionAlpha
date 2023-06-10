using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class LabPlatingWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Panel Wall"); 
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<LabPlatingWallUnsafe2>();
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<LabPlatingWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<LabPlating>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}