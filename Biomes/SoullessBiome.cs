using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Items.Accessories.HM;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Biomes
{
    public class SoullessBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/SoullessWaterStyle");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/SoullessUndergroundBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/SoullessCaverns2");

        public override string BestiaryIcon => "Textures/Bestiary/SoullessCaverns";
        public override string BackgroundPath => "Textures/MapBackgrounds/SoullessCavernsMap";
        public override Color? BackgroundColor => base.BackgroundColor;
        public override void SpecialVisuals(Player player)
        {
            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1f).UseIntensity(1f).UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", player.InModBiome(ModContent.GetInstance<SoullessBiome>()));
            player.ManageSpecialBiomeVisuals("MoR:SoullessSky", player.InModBiome(ModContent.GetInstance<SoullessBiome>()), player.Center);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", false);
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulless Caverns");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().SoullessTileCount >= 200;
        }
    }
}