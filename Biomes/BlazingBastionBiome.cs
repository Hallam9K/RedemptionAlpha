using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class BlazingBastionBiome : ModBiome
    {
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override SceneEffectPriority Priority => SceneEffectPriority.None;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Bastion");
        }
        public override void OnInBiome(Player player)
        {
            BastionArea.Active = true;
        }
        public override bool IsBiomeActive(Player player)
        {
            return player.ZoneUnderworldHeight && player.Center.X > (Main.maxTilesX - 350) * 16;
        }
    }
}