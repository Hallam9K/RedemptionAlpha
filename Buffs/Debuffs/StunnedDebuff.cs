using Microsoft.Xna.Framework;
using Redemption.Globals.NPC;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class StunnedDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stunned!");
			Description.SetDefault("You are stunned!");
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

            player.velocity.Y += player.gravity;
            if (player.velocity.Y > player.maxFallSpeed)
            {
                player.velocity.Y = player.maxFallSpeed;
            }
            player.sandStorm = false;
            player.canJumpAgain_Cloud = false;
            player.canJumpAgain_Sandstorm = false;
            player.canJumpAgain_Blizzard = false;
            player.canJumpAgain_Fart = false;
            player.canJumpAgain_Sail = false;
            player.canJumpAgain_Unicorn = false;
            if (player.mount.Active)
                player.mount.Dismount(player);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<BuffNPC>().stunned = true;
        }
    }
}