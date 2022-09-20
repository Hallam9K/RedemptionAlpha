using Terraria.Audio;
using Redemption.BaseExtension;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals.Player;

namespace Redemption.Items.Accessories.HM
{
    public class PocketShieldGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pocket-Shield Generator");
            Tooltip.SetDefault("Summons a bubble shield that can protect the user from a high amount of damage before breaking\n" +
                "However, on the impact that breaks the shield, the user will receive 4x the damage it took in said impact\n" +
                "Once broken, has a 1 minute cooldown");
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
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            BuffPlayer bP = player.GetModPlayer<BuffPlayer>();
            if (bP.shieldGeneratorCD <= 0)
            {
                if (bP.shieldGeneratorAlpha <= 0)
                    SoundEngine.PlaySound(CustomSounds.ShieldActivate, player.position);
                if (bP.shieldGeneratorAlpha < 0.6f)
                    bP.shieldGeneratorAlpha += 0.04f;
                bP.shieldGenerator = true;
            }
            else
            {
                bP.shieldGeneratorAlpha = 0;
                bP.shieldGeneratorCD--;
            }
        }
    }
}
