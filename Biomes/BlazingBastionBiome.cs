using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class BlazingBastionBiome : ModBiome
    {
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override int Music => MusicID.OtherworldlyUnderworld;
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blazing Bastion");
        }
        public override bool IsBiomeActive(Player player)
        {
            if (SubworldSystem.Current != null)
                return false;
            return player.ZoneUnderworldHeight && player.Center.X > (Main.maxTilesX - 350) * 16;
        }
    }
}