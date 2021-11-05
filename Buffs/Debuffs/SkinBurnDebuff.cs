using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class SkinBurnDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skin Burn");
			Description.SetDefault("Your skin is burning!");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            CanBeCleared = false;
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