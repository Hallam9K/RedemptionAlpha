using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabWallFan : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Wall Fan");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabWallFanTile>(), 0);
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 99;
            Item.value = 800;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 7)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}