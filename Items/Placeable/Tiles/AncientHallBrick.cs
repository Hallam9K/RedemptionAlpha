using Redemption.Tiles.Tiles;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientHallBrick : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("[c/ff0000:Unbreakable (500% Pickaxe Power)]");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientHallBrickTile>(), 0);
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
		}
    }
}
