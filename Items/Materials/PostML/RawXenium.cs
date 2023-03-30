using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Materials.PostML
{
    public class RawXenium : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Use a Xenium Refinery to craft Xenium Bars");
            Item.ResearchUnlockCount = 50;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
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