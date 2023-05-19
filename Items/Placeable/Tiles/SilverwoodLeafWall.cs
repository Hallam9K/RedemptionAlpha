using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Walls;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class SilverwoodLeafWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Living Silverwood Leaf Wall");
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<SilverwoodLeafWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<Silverwood>()
                .AddTile(TileID.LivingLoom)
                .Register();
        }
    }
}