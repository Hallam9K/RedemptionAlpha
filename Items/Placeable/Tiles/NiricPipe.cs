using Redemption.Globals;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Placeable.Tiles
{
    public class NiricPipe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Niric Brass Pipe");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<NiricPipeTile>(), 0);
            Item.width = 20;
            Item.height = 16;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}