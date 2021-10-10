using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class EldritchRootBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eldritch Regen");
            Description.SetDefault("Life regen greatly increased");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 3;
        }
    }
}