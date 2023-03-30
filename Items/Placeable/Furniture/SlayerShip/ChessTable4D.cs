using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class ChessTable4D : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("4D Chess Table");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<ChessTable4DTile>(), 0);
            Item.width = 44;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 5500;
            Item.rare = ItemRarityID.LightPurple;
		}
    }
}