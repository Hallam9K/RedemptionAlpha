using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class LabBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/WastelandWaterStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabMusic");

        public override string BestiaryIcon => "Textures/Bestiary/TeochromeIcon";
		public override string BackgroundPath => "Textures/MapBackgrounds/LabMapBackground";
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abandoned Laboratory");
        }

        public override void OnInBiome(Player player)
        {
            LabArea.Active = true;
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().LabTileCount >= 100;
        }
    }
}