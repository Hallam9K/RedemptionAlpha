using Redemption.Tiles.Furniture.Shade;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneTable2 : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Long Shadestone Table");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneTable2Tile>(), 0);
            Item.width = 58;
            Item.height = 24;
            Item.maxStack = 99;
            Item.value = 500;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}