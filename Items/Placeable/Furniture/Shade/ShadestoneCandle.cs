using Redemption.Tiles.Furniture.Shade;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneCandle : ModItem
	{
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneCandleTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue; 
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 4)
                .AddIngredient(ItemID.Torch)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}