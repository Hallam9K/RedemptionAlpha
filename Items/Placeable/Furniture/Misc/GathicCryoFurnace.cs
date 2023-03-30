using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Redemption.Tiles.Furniture.Misc;
using Redemption.Globals;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class GathicCryoFurnace : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gathic Cryo-Furnace");
            // Tooltip.SetDefault("Used for smelting ore");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GathicCryoFurnaceTile>(), 0);
            Item.width = 34;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 0, 60);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RedeRecipe.GathicStoneRecipeGroup, 20)
                .AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 4)
                .AddIngredient(ItemID.IceTorch, 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}