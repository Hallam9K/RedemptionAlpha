using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class RadiationDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Irradiated");
			Description.SetDefault("You are dying of radiation!");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            CanBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;

            player.lifeRegen = -50;
            player.bleed = true;
            player.blackout = true;
            player.blind = true;
        }
    }
}