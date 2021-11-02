using Redemption.Tiles.Furniture.Archcloth;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Archcloth
{
    public class ArchclothBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ArchclothBannerTile>(), 0);
            Item.width = 12;
            Item.height = 28;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Materials.PreHM.Archcloth>(), 3)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}