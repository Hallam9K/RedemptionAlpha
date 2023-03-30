using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;
using Redemption.Rarities;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShinkiteBrickWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<ShinkiteBrickWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
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