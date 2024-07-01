using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class HardenedSludgeWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<HardenedSludgeWallTileSafe>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<HardenedSludgeSafe>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}