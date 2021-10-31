using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GathicStoneTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicStoneWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock)
                .AddIngredient(ItemID.AshBlock, 5)
                .AddTile(TileID.Solidifier)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicGladestone>())
                .Register();
        }
    }
}
