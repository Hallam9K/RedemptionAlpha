using Microsoft.Xna.Framework;
using Redemption.WorldGeneration.Space;
using SubworldLibrary;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals;

namespace Redemption.Biomes
{
    public class SpaceBiome : ModBiome
    {
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Redemption/SpaceBgStyle");
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/DusksEdge");
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Space");
        }
        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (isActive)
                SkyManager.Instance.Activate("MoR:SpaceSky");
        }
        public override void OnInBiome(Player player)
        {
            SpaceArea.Active = true;
        }
        public override void OnLeave(Player player)
        {
            SkyManager.Instance.Deactivate("MoR:SpaceSky");
        }
        public override bool IsBiomeActive(Player player)
        {
            return SubworldSystem.IsActive<SpaceSub>();
        }
    }
}