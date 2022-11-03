using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Containers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class StandardLabLocker : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Standard Security Locker");
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<StandardLabLockerTile>(), 0);
			Item.width = 30;
			Item.height = 24;
			Item.maxStack = 9999;
			Item.value = 5000;
			Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<LabPlating>(), 8)
				.AddTile(TileID.WorkBenches)
				.Register();
        }
    }
}