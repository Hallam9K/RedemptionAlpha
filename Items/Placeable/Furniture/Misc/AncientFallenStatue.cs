using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class AncientFallenStatue : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Ancient Fallen Statue");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientFallenStatueTile>(), 0);
            Item.width = 40;
            Item.height = 38;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 0, 75, 0);
        }
    }
}