using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShinkiteBrickOrnate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ornate Shinkite Brick");
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShinkiteBrickOrnateTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ShinkiteBrickOrnateWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
