using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
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

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/SoullessCaverns";
        public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/SoullessCavernsMap";
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath;
        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
            {
                Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1f + player.Redemption().visionAmt).UseIntensity(0.9f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            }
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", isActive && SubworldSystem.IsActive<SoullessSub>());
            player.ManageSpecialBiomeVisuals("MoR:SoullessSky", isActive && !player.RedemptionPlayerBuff().dreamsong, player.Center);
        }
        public override void OnLeave(Player player)
        {
            Terraria.Graphics.Effects.Filters.Scene["MoonLordShake"].Deactivate();
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulless Caverns");
        }
        public override void OnInBiome(Player player)
        {
            player.RedemptionAbility().SpiritwalkerActive = false;
            Lighting.AddLight(player.Center, 1.5f, 1.5f, 1.5f);
            if (player.HasBuff<StunnedDebuff>())
                player.maxFallSpeed += 4;
            else
                player.maxFallSpeed += 2;
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().SoullessTileCount >= 200 && SubworldSystem.IsActive<SoullessSub>();
        }
    }
}
