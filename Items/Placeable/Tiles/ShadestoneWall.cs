using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Walls;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadestoneWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<ShadestoneWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 999;
			Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}