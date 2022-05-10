using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.WorldGeneration.Soulless;
using ReLogic.Content;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Biomes
{
    public class SoullessBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/SoullessWaterStyle");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/SoullessUndergroundBackgroundStyle");

        public override int Music => Main.LocalPlayer.RedemptionPlayerBuff().dreamsong ? MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SoullessCaverns") : MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SoullessCaverns2");

        public override string BestiaryIcon => "Textures/Bestiary/SoullessCaverns";
        public override string BackgroundPath => "Textures/MapBackgrounds/SoullessCavernsMap";
        public override Color? BackgroundColor => base.BackgroundColor;
        public override void SpecialVisuals(Player player)
        {
            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1f).UseIntensity(0.9f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", player.InModBiome(ModContent.GetInstance<SoullessBiome>()));
            player.ManageSpecialBiomeVisuals("MoR:SoullessSky", player.InModBiome(ModContent.GetInstance<SoullessBiome>()) && !player.RedemptionPlayerBuff().dreamsong, player.Center);
        }
        public override void OnLeave(Player player)
        {
            Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].Deactivate();
            player.ManageSpecialBiomeVisuals("MoR:SoullessSky", false, player.Center);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", false);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulless Caverns");
        }
        public override void OnInBiome(Player player)
        {
            SoullessArea.Active = true;
            Lighting.AddLight(player.Center, 0.5f, 0.5f, 0.5f);
            player.maxFallSpeed += 4;
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().SoullessTileCount >= 200 && SubworldSystem.IsActive<SoullessSub>();
        }
    }
}
