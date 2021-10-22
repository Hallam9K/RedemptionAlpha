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
                player.velocity.Y *= 0.9f;

            player.moveSpeed *= 0.5f;
        }
    }
}