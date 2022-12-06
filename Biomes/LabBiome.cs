using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class LabBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/WastelandWaterStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabMusic");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/TeochromeIcon";
		public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/LabMapBackground";
		public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abandoned Laboratory");
        }
        public override void OnEnter(Player player)
        {
            if (!player.Redemption().foundLab)
                player.Redemption().foundLab = true;
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().LabTileCount >= 100;
        }
    }
}