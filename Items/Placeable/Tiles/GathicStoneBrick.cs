using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicStoneBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GathicStoneBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicStoneBrickWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicStone>(), 2)
                .AddTile(TileID.Furnaces)
                .Register();
            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<AncientHallPillarWall>())
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }
    }
}
