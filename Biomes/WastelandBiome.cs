using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.HM;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
namespace Redemption.Biomes
{
    public static class WastelandEffect
    {
        public static void SpecialVisuals(Player player)
        {
            bool fogSafe = BasePlayer.HasAccessory(player, ModContent.ItemType<GasMask>(), true, false); //|| BasePlayer.HasAccessory(player, ModContent.ItemType<HEVSuit>(), true, false);
            player.ManageSpecialBiomeVisuals("MoR:WastelandSky", player.InModBiome(ModContent.GetInstance<WastelandPurityBiome>()), player.Center);

            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(fogSafe ? 0.25f : 0.3f).UseIntensity(fogSafe ? 0.6f : 1f)
                .UseColor(Color.DarkOliveGreen).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Perlin", AssetRequestMode.ImmediateLoad).Value);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", player.InModBiome(ModContent.GetInstance<WastelandPurityBiome>()));
        }
        public static void OnInBiome(Player player, Mod mod)
        {
            if (Main.raining)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(500) && !Main.dedServ)
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(mod, "Sounds/Custom/Muller1").WithVolume(.9f).WithPitchVariance(.1f), player.position);

                if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
                    player.AddBuff(ModContent.BuffType<HeavyRadiationDebuff>(), 30);
                else
                    player.AddBuff(ModContent.BuffType<RadioactiveFalloutDebuff>(), 30);

                if (Main.rand.Next(80000) == 0 && player.GetModPlayer<Radiation>().irradiatedLevel == 0) //&& !HEVPower && !hazmatPower)
                    player.GetModPlayer<Radiation>().irradiatedLevel++;
            }
            else
                player.AddBuff(ModContent.BuffType<RadioactiveFalloutDebuff>(), 30);
        }
    }
    public class WastelandPurityBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/WastelandWaterStyle");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/WastelandUndergroundBackgroundStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Redemption/WastelandSurfaceBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Wasteland");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/Wasteland";
        public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/PurityWastelandMap1";
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsPrimaryBiome => true;
        public override void SpecialVisuals(Player player)
        {
            WastelandEffect.SpecialVisuals(player);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("MoR:WastelandSky", false, player.Center);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", false);
        }
        public override void OnInBiome(Player player)
        {
            WastelandEffect.OnInBiome(player, Mod);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wasteland");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().WastelandTileCount >= 100;
        }
    }
    public class WastelandSnowBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/WastelandWaterStyle");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/WastelandSnowUndergroundBackgroundStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Redemption/WastelandSnowBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Wasteland");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/WastelandSnow";
        public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/SnowWastelandMap1";
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsPrimaryBiome => false;
        public override void SpecialVisuals(Player player)
        {
            WastelandEffect.SpecialVisuals(player);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("MoR:WastelandSky", false, player.Center);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", false);
        }
        public override void OnInBiome(Player player)
        {
            WastelandEffect.OnInBiome(player, Mod);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Taiga Wasteland");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().WastelandSnowTileCount >= 50;
        }
    }
    public class WastelandDesertBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/WastelandWaterStyle");

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Redemption/WastelandDesertBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Wasteland");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/WastelandDesert";
        public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/DesertWastelandMap1";
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsPrimaryBiome => false;
        public override void SpecialVisuals(Player player)
        {
            WastelandEffect.SpecialVisuals(player);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("MoR:WastelandSky", false, player.Center);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", false);
        }
        public override void OnInBiome(Player player)
        {
            WastelandEffect.OnInBiome(player, Mod);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Wasteland");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().WastelandDesertTileCount >= 50;
        }
    }
    public class WastelandCorruptionBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/WastelandWaterStyle");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/WastelandCorruptUndergroundBackgroundStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Redemption/WastelandCorruptBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Wasteland");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/WastelandCorrupt";
        public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/CorruptionWastelandMap1";
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsPrimaryBiome => false;
        public override void SpecialVisuals(Player player)
        {
            WastelandEffect.SpecialVisuals(player);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("MoR:WastelandSky", false, player.Center);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", false);
        }
        public override void OnInBiome(Player player)
        {
            WastelandEffect.OnInBiome(player, Mod);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corrupt Wasteland");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().WastelandCorruptTileCount >= 50;
        }
    }
    public class WastelandCrimsonBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Redemption/WastelandWaterStyle");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Redemption/WastelandCrimsonUndergroundBackgroundStyle");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Redemption/WastelandCrimsonBackgroundStyle");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Wasteland");

        public override string BestiaryIcon => "Redemption/Textures/Bestiary/WastelandCrimson";
        public override string BackgroundPath => "Redemption/Textures/MapBackgrounds/CrimsonWastelandMap1";
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsPrimaryBiome => false;
        public override void SpecialVisuals(Player player)
        {
            WastelandEffect.SpecialVisuals(player);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("MoR:WastelandSky", false, player.Center);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", false);
        }
        public override void OnInBiome(Player player)
        {
            WastelandEffect.OnInBiome(player, Mod);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Wasteland");
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<RedeTileCount>().WastelandCrimsonTileCount >= 50;
        }
    }
}