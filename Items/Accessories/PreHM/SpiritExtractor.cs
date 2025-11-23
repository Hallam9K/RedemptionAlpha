using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class SpiritExtractor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().cruxSpiritExtractor = true;
        }
        public override void UpdateVanity(Player player)
        {
            player.RedemptionPlayerBuff().cruxSpiritExtractor = true;
        }
    }
}