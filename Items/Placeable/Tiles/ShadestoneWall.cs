using Terraria.ModLoader;
using Redemption.Walls;
using Terraria.GameContent.Creative;
using Redemption.Rarities;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadestoneWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 400;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlacableWall((ushort)ModContent.WallType<ShadestoneWallTile>());
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = 9999;
			Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}