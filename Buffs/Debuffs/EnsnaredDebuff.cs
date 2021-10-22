using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

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
            player.GetModPlayer<BuffPlayer>().ensnared = true;
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