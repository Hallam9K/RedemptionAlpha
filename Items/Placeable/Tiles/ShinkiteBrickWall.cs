using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShinkiteBrickWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlacableWall((ushort)ModContent.WallType<ShinkiteBrickWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<ShinkiteBrick>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}