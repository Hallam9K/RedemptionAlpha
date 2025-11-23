using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class ForestCore : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.NatureS);

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 6, 0, 0);
            Item.hasVanityEffects = true;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.forestCore = true;
        }
    }
}