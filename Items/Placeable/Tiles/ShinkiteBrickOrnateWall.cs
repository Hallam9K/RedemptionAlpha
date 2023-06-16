using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Walls;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShinkiteBrickOrnateWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ornate Shinkite Brick Wall");
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<ShinkiteBrickOrnateWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<ShinkiteBrickOrnate>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}