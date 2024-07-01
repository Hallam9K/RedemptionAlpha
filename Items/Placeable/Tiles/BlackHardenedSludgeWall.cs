using Redemption.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class BlackHardenedSludgeWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<BlackHardenedSludgeWallTileSafe>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<BlackHardenedSludgeSafe>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}