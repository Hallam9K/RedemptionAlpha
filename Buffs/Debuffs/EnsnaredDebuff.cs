using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class EnsnaredDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ensnared");
            Description.SetDefault("Tangled in thorns");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().ensnared = true;
            if (player.velocity.Y < 0)
                player.velocity.Y *= 0.6f;
            if (player.velocity.Y < -2)
            {
                player.velocity.Y = 0;
                player.jump = 0;
            }

            player.velocity.X *= 0.8f;
        }
    }
}