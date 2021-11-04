using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public class WastelandBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/WastelandWaterStyle");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/WastelandUndergroundBackgroundStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Redemption/WastelandSurfaceBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Wasteland");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/Wasteland";
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsPrimaryBiome => true;
        public override void SpecialVisuals(Player player)
        {
            //bool fogSafe = BasePlayer.HasAccessory(player, ModContent.ItemType<GasMask>(), true, false) || BasePlayer.HasAccessory(player, ModContent.ItemType<HEVSuit>(), true, false);
            player.ManageSpecialBiomeVisuals("MoR:WastelandSky", player.InModBiome(ModContent.GetInstance<WastelandBiome>()), player.Center);

            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.3f).UseIntensity(1f)
                .UseColor(Color.DarkOliveGreen).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Perlin", AssetRequestMode.ImmediateLoad).Value);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", player.InModBiome(ModContent.GetInstance<WastelandBiome>()));
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("MoR:WastelandSky", false, player.Center);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", false);
        }
        /*public override void OnInBiome(Player player)
        {
            player.AddBuff(ModContent.BuffType<RadioactiveFalloutDebuff>(), 30);
            if (player.wet && !player.lavaWet && !player.honeyWet) // TODO: && !labWaterImmune)
            {
                if (player.lifeRegen > 10)
                    player.lifeRegen = 10;

                player.lifeRegenTime = 0;
                player.lifeRegen -= 60;
            }
        }*/

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wasteland");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().WastelandTileCount >= 100;
        }
    }
}