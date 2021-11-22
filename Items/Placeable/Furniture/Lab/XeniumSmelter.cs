using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class XeniumSmelter : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Used to melt scraps of metal"
                + "\nFound in the Abandoned Lab");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<XeniumSmelterTile>(), 0);
			Item.width = 30;
			Item.height = 36;
			Item.maxStack = 99;
			Item.value = Item.sellPrice(0, 10, 0, 0);
			Item.rare = ItemRarityID.Purple;
		}
	}
}