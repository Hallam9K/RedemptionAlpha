using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Containers;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class LabChest : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("High Security Laboratory Crate");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LabChestTileLocked>(), 0);
			Item.width = 30;
			Item.height = 28;
			Item.maxStack = 9999;
			Item.value = 5000;
			Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<LabPlating>(), 8)
				.AddIngredient(ModContent.ItemType<Plating>(), 2)
				.AddTile(TileID.WorkBenches)
				.Register();
        }
    }
}