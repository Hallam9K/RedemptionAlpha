using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class SkinBurnDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;

            player.lifeRegen = -3;
            player.bleed = true;
            player.blind = true;
        }
    }
}