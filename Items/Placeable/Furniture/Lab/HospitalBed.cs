using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class HospitalBed : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Hospital Bed");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<HospitalBedTile>(), 0);
            Item.width = 38;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = 6000;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 12)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 8)
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}