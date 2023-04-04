using Terraria.Audio;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals.Player;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Accessories.HM
{
    public class PocketShieldGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pocket-Shield Generator");
            /* Tooltip.SetDefault("Summons a bubble shield that can protect the user from a high amount of damage before breaking\n" +
                "However, on the impact that breaks the shield, the user will receive 2x the damage it took in said impact\n" +
                "Once broken, has a 1 minute cooldown\n" +
                "While an Energy Pack is in your inventory, the shield will restore 4% of its life at the cost of 1% Energy per second"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 9));
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
        }
        private int timer;
        public override void UpdateInventory(Player player)
        {
            BuffPlayer bP = player.GetModPlayer<BuffPlayer>();
            bP.shieldGeneratorCD = (int)MathHelper.Max(60, bP.shieldGeneratorCD);
        }
        public override void HoldItem(Player player)
        {
            BuffPlayer bP = player.GetModPlayer<BuffPlayer>();
            bP.shieldGeneratorCD = (int)MathHelper.Max(60, bP.shieldGeneratorCD);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            timer++;
            BuffPlayer bP = player.GetModPlayer<BuffPlayer>();
            EnergyPlayer eP = player.GetModPlayer<EnergyPlayer>();
            if (bP.shieldGeneratorCD <= 0)
            {
                if (bP.shieldGeneratorAlpha <= 0)
                    SoundEngine.PlaySound(CustomSounds.ShieldActivate, player.position);
                if (bP.shieldGeneratorAlpha < 0.6f)
                    bP.shieldGeneratorAlpha += 0.04f;
                bP.shieldGenerator = true;

                if (eP.energyMax > 0 && eP.statEnergy >= (int)(eP.energyMax * 0.01f) && bP.shieldGeneratorLife < 200)
                {
                    eP.stopEnergyRegen = true;
                    if (timer % 60 == 0)
                    {
                        eP.statEnergy -= (int)(eP.energyMax * 0.01f);
                        bP.shieldGeneratorLife += 8;
                    }
                }
            }
            else
            {
                bP.shieldGeneratorAlpha = 0;
                bP.shieldGeneratorCD--;
            }
        }
    }
}
