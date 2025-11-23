using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Globals.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class HeartInsignia : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ShadowS, ElementID.BloodS);

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = Request<Texture2D>(Texture + "_Glow").Value;
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
