using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Items.Usable;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class EnergizedGathicStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<GathicStone>();
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<EnergizedGathicStoneTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
        }
    }
}
