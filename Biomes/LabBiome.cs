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

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/LabUndergroundBackgroundStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Redemption/LabSurfaceBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/LabMusic");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/Laboratory";
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abandoned Lab");
        }

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().LabTileCount >= 10;
        }
    }
}