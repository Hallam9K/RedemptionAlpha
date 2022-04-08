using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientAlloyPipe : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientAlloyPipeTile>(), 0);
            Item.width = 20;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            // TODO: Alloy pipe recipe
        }
    }
}
