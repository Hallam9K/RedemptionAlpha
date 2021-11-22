using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabPlatform : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Platform");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<LabPlatformTile>(), 0);
            Item.width = 28;
            Item.height = 14;
            Item.maxStack = 99;
            Item.value = 100;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe(2)
               .AddIngredient(ModContent.ItemType<LabPlating>())
               .AddTile(TileID.WorkBenches)
               .Register();
        }
    }
}