using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria.Localization;

namespace Redemption.Items.Accessories.PreHM
{
    public class HeartInsignia : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ShadowS, ElementID.BloodS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heart Insignia");
            /* Tooltip.SetDefault("Picking up hearts give a short boost to life regen" +
                "\n15% increased " + ElementID.ShadowS + " and " + ElementID.BloodS + " elemental damage and resistance"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();

            modPlayer.ElementalDamage[ElementID.Shadow] += 0.15f;
            modPlayer.ElementalDamage[ElementID.Blood] += 0.15f;

            modPlayer.ElementalResistance[ElementID.Shadow] += 0.15f;
            modPlayer.ElementalResistance[ElementID.Blood] += 0.15f;

            modPlayer.heartInsignia = true;
        }
    }
}
