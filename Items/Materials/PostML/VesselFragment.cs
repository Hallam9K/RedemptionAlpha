using Redemption.Rarities;
using Redemption.Tiles.Ores;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class VesselFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Vessel Fragment");
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MasksTile>(), 0);
            Item.width = 20;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 85, 0);
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}