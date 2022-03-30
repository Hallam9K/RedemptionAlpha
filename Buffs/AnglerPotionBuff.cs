using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class AnglerPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angler Vision");
            Description.SetDefault("Increases damage and emits light while submerged");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().anglerPot = true;
            if (player.wet && !player.lavaWet)
            {
                player.GetDamage(DamageClass.Generic) += 0.15f;
                player.accFlipper = true;
                Lighting.AddLight(player.Center, 2f, 2f, 2f);
            }
        }
    }
}