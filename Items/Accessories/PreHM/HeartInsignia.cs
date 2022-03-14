using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals.Player;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Items.Accessories.PreHM
{
    public class HeartInsignia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart Insignia");
            Tooltip.SetDefault("Picking up hearts give a short boost to life regen" +
                "\n15% increased Shadow and Blood elemental damage and resistance");
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
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer modPlayer = player.RedemptionPlayerBuff();

            modPlayer.ElementalDamage[8] += 0.15f;
            modPlayer.ElementalDamage[11] += 0.15f;

            modPlayer.ElementalResistance[8] += 0.15f;
            modPlayer.ElementalResistance[11] += 0.15f;

            modPlayer.heartInsignia = true;
        }
    }
}
