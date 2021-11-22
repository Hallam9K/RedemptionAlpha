using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class Vent : ModItem
	{
        public override void SetStaticDefaults()
        {
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<VentTile>(), 0);
			Item.width = 32;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = 100;
			Item.rare = ItemRarityID.LightPurple;
		}
	}
}