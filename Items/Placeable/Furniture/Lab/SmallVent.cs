using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class SmallVent : ModItem
	{
        public override void SetStaticDefaults()
        {
			SacrificeTotal = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SmallVentTile>(), 0);
			Item.width = 32;
			Item.height = 16;
			Item.maxStack = 9999;
			Item.value = 100;
			Item.rare = ItemRarityID.LightPurple;
		}
	}
}