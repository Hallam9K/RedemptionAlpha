using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class StaticStunDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Static Shock!");
			// Description.SetDefault("You are stunned by electricity!");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.wingTime = 0;
            player.velocity.X += Main.rand.Next(-4, 5);
            player.velocity.Y += Main.rand.Next(-4, 5);
            player.velocity.X = MathHelper.Clamp(player.velocity.X, -10, 10);
            player.velocity.Y = MathHelper.Clamp(player.velocity.X, -10, 10);
            player.position = player.oldPosition;
        }
    }
}