using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class GraniteAuraBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Granite Aura");
            // Description.SetDefault("You seep with energy");
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.15f;
            player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.15f;
        }
    }
}