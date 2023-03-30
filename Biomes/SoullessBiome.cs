using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Biomes
{
    public class SoullessBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/SoullessWaterStyle");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/SoullessUndergroundBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SoullessCaverns2");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/SoullessCaverns";
        public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/SoullessCavernsMap";
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath;
        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
                Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1f).UseIntensity(1f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", isActive);
            player.ManageSpecialBiomeVisuals("MoR:SoullessSky", isActive, player.Center);
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soulless Caverns");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().SoullessTileCount >= 200;
        }
    }
}