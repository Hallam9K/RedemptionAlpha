using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class ExplosiveFlaskBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Weapon Imbue: Nitroglycerine");
			// Description.SetDefault("Melee attacks gain the Explosive bonus");
			Main.persistentBuff[Type] = true;
			Main.meleeBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.dead || !player.active)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
