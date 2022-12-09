using Redemption.Tiles.Furniture.Shade;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneMirror : ModItem
	{
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneMirrorTile>(), 0);
            Item.width = 18;
            Item.height = 28;
            Item.maxStack = 99;
            Item.value = 700;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 10)
                .AddIngredient(ItemID.Glass, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}