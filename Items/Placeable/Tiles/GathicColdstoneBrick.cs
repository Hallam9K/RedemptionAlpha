using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class GathicColdstoneBrick : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GathicColdstoneBrickTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<GathicColdstone>(), 2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
