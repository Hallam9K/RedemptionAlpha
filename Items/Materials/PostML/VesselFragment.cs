using Redemption.Rarities;
using Redemption.Tiles.Ores;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class VesselFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vessel Fragment");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MasksTile>(), 0);
            Item.width = 20;
            Item.height = 18;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 85, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}