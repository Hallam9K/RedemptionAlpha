using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Cooldowns
{
	public class CruxCardCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
			BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (RedeHelper.BossActive())
            {
                Main.buffNoTimeDisplay[Type] = true;
                player.buffTime[buffIndex] = (int)MathHelper.Max(player.buffTime[buffIndex], 2);
            }
            else
                Main.buffNoTimeDisplay[Type] = false;
        }
    }
}