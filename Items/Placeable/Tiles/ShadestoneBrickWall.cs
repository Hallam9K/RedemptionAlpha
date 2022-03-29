using Terraria.ModLoader;
using Redemption.Walls;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadestoneBrickWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("[c/ff0000:Unbreakable]");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<ShadestoneBrickWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 999;
			Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}