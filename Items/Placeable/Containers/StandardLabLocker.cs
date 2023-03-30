using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Containers;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class StandardLabLocker : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Standard Security Locker");
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<StandardLabLockerTile>(), 0);
			Item.width = 30;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
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