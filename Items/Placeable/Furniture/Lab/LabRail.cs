using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabRail_L : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Railing (Left)");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabRailTile_L>(), 0);
            Item.width = 16;
            Item.height = 26;
            Item.maxStack = 99;
            Item.value = 200;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
               .AddIngredient(ModContent.ItemType<LabPlating>())
               .AddIngredient(ItemID.Glass)
               .AddTile(TileID.WorkBenches)
               .Register();
        }
    }
    public class LabRail_Mid : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Railing (Middle)");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabRailTile_Mid>(), 0);
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = 99;
            Item.value = 200;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<LabPlating>())
                .AddIngredient(ItemID.Glass)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    public class LabRail_R : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Railing (Right)");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabRailTile_R>(), 0);
            Item.width = 16;
            Item.height = 26;
            Item.maxStack = 99;
            Item.value = 200;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient(ModContent.ItemType<LabPlating>())
                .AddIngredient(ItemID.Glass)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}