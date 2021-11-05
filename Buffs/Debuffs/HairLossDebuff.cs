using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
	public class HairLossDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hair Loss");
			Description.SetDefault("Your hair is gone!");
			Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            CanBeCleared = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().hairLoss = true;
        }
    }
}