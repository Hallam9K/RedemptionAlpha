using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class ShockDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().shockDebuff = true;
            player.lifeRegen -= 100;
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
                player.velocity.Y = player.maxFallSpeed;

            player.sandStorm = false;
            player.blockExtraJumps = true;
            if (player.mount.Active)
                player.mount.Dismount(player);

            Lighting.AddLight(player.Center, Color.LimeGreen.ToVector3() * 0.9f);
        }
    }
}