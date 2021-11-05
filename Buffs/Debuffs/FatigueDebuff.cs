using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class FatigueDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fatigue");
			Description.SetDefault("You are overwhelmed with tiredness.");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.blackout = true;
            player.blind = true;
            player.moveSpeed *= 0.4f;
        }
	}
}