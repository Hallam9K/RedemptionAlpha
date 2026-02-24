using Redemption.Items.Accessories.HM;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.Cooldowns
{
    public class MicroshieldCoreCooldown : ModBuff
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
            MicroshieldCore_Player modPlayer = player.GetModPlayer<MicroshieldCore_Player>();
            if (modPlayer.shieldDisabled && player.ownedProjectileCounts[ProjectileType<MicroshieldDrone>()] > 0)
            {
                int time = (int)MathHelper.Lerp(MicroshieldDrone.RESTORE_TIME, 0, modPlayer.restoreTimer / (float)MicroshieldDrone.RESTORE_TIME);
                player.buffTime[buffIndex] = time;
            }
            else
            {
                if (player.buffTime[buffIndex] > 2)
                    player.buffTime[buffIndex] = 2;
            }
        }
    }
}
