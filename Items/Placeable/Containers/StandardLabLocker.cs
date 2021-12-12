using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Containers;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class StandardLabLocker : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Standard Security Locker");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<StandardLabLockerTile>(), 0);
			Item.width = 30;
			Item.height = 24;
			Item.maxStack = 99;
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