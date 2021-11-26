using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Buffs.Debuffs
{
    public class HazardLaserDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Incinerated!");
			Description.SetDefault("You are being lasered!");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
            player.lifeRegen = -1500;
            Dust.NewDust(new Vector2(player.position.X - player.velocity.X * 2f, player.position.Y - 2f - player.velocity.Y * 2f), player.width, player.height, DustID.LifeDrain, 0f, 0f, 100, default, 2f);
        }
    }
}