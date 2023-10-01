using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class Stunned2Debuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.wingTimeMax = 0;
            player.wingTime = 0;
            player.wings = 0;
            player.wingsLogic = 0;
            player.noFallDmg = true;
            player.noBuilding = true;

            player.controlJump = false;
            player.controlDown = false;
            player.controlLeft = false;
            player.controlRight = false;
            player.controlUp = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlThrow = false;
            player.gravDir = 1f;

            player.velocity *= 0;
            player.position = player.oldPosition;
            player.sandStorm = false;
            player.blockExtraJumps = true;
            if (player.mount.Active)
                player.mount.Dismount(player);
        }
    }
}