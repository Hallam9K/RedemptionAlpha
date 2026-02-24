using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class ObsidianStatuette : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<ObsidianStatuetteTile>(), 0);
            Item.width = 24;
            Item.height = 34;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 0, 80);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AshWood, 6)
                .AddIngredient(ItemID.Obsidian, 2)
                .AddTile(TileID.Sawmill)
                .Register();
        }
    }
}