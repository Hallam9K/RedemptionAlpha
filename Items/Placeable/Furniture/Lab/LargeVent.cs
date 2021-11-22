using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LargeVent : ModItem
	{
        public override void SetStaticDefaults()
        {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LargeVentTile>(), 0);
			Item.width = 48;
			Item.height = 32;
			Item.maxStack = 99;
			Item.value = 100;
			Item.rare = ItemRarityID.LightPurple;
		}
	}
}