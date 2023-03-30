using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class SlayerShipBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SlayerShipMusic");

        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slayer's Crashed Cruiser");
        }

        public override bool IsBiomeActive(Player player)
        {
            return RedeBossDowned.downedSlayer && ModContent.GetInstance<RedeTileCount>().SlayerShipTileCount >= 75;
        }
    }
}