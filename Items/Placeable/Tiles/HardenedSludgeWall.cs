using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class HardenedSludgeWall : ModItem
    {
        public override void SetDefaults()
		{
            Item.DefaultToPlacableWall((ushort)ModContent.WallType<HardenedSludgeWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.Purple;
        }
    }
}