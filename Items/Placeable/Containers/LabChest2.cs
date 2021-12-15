using Redemption.Tiles.Containers;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class LabChest2 : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Special Laboratory Chest");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LabChestTileLocked2>(), 0);
			Item.width = 30;
			Item.height = 28;
			Item.maxStack = 99;
			Item.value = 5000;
			Item.rare = ItemRarityID.LightPurple;
		}
    }
}