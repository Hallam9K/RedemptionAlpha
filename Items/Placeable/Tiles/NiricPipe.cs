using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Placeable.Tiles
{
    public class NiricPipe : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Niric Brass Pipe");
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<NiricPipeTile>(), 0);
            Item.width = 20;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}