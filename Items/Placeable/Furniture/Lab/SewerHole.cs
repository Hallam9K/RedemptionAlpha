using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class SewerHole : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DrawUnsafeIndicator[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SewerHoleTile>(), 0);
            Item.width = 26;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}