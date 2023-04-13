using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class EldritchRoot : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.NatureS, ElementID.ShadowS);
        public override void SetStaticDefaults()
        {
            /*Tooltip.SetDefault("All " + ElementID.NatureS + " elemental weapons gain the " + ElementID.ShadowS + " element\n" +
                "Your " + ElementID.ShadowS + " weapons are more effective against Dark enemies" +
                "\n12% increased " + ElementID.ShadowS + " elemental resistance\n" +
                "'Nature can reap, too.'");*/
            ElementID.ItemNature[Type] = true;
            ElementID.ItemShadow[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();
            modPlayer.eldritchRoot = true;
            modPlayer.ElementalResistance[ElementID.Shadow] += 0.12f;
        }
    }
}