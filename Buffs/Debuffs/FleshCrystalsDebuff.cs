using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Redemption.Buffs.Debuffs
{
    public class FleshCrystalsDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flesh Crystals");
            Description.SetDefault("...The pain... It's... Unbearable");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().fleshCrystals = true;
            player.statDefense -= 11;
            player.moveSpeed *= 0.70f;
            player.AddBuff(ModContent.BuffType<HemorrhageDebuff>(), 1800);
            player.AddBuff(ModContent.BuffType<NecrosisDebuff>(), 3600);
            Lighting.AddLight(player.Center, Color.LimeGreen.ToVector3() * 0.7f);
        }
    }
}