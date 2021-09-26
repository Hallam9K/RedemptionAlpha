using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Redemption.Buffs.Debuffs
{
    public class HemorrhageDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hemhorraging");
            Description.SetDefault("The crystals... They're sharp... It hurts to move...");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().hemorrhageDebuff = true;
            if (player.velocity != Vector2.Zero)
            {
                player.lifeRegen -= 25;
            }
        }
    }
}