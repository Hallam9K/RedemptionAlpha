using Redemption.Items.Placeable.Furniture.AncientWood;
using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientWood : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 50;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<AncientWoodTile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood)
                .AddIngredient(ItemID.AshBlock, 5)
                .AddTile(TileID.Solidifier)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AncientWoodWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AncientWoodPlatform>(), 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
