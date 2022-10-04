using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Materials.PostML
{
    public class RawXenium : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Use a Xenium Refinery to craft Xenium Bars");
            SacrificeTotal = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 15, 0);
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<XenomiteShard>(), 2)
                .AddIngredient(ItemID.LunarOre)
                .AddTile(ModContent.TileType<XeniumRefineryTile>())
                .Register();
        }
    }
}