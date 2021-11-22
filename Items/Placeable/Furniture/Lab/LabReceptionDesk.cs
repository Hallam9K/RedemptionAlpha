using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabReceptionDesk : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Reception Desk");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabReceptionDeskTile>(), 0);
            Item.width = 34;
            Item.height = 26;
            Item.maxStack = 99;
            Item.value = 1400;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 18)
                .AddIngredient(ModContent.ItemType<LabComputer>())
                .AddIngredient(ModContent.ItemType<Plating>(), 3)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}