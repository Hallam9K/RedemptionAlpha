using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals.Player;
using ReLogic.Content;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Globals
{
    public class SilenceEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/silence");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return RedeSystem.Silence || player.RedemptionRad().irradiatedEffect == 2;
        }
    }
    public class SkeletonInvasionMusic : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Spooky");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return RedeWorld.SkeletonInvasion && player.ZoneOverworldHeight;
        }
    }
    public class NukeMusic : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Warhead");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return RedeWorld.nukeCountdownActive;
        }
    }
    public class Rad1Music : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Rad1");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.RedemptionRad().irradiatedEffect == 3;
        }
    }
    public class Rad2Music : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Rad2");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.RedemptionRad().irradiatedEffect >= 4;
        }
    }
    public class DancingSkeletonEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Island");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Terraria.Player player, bool isActive)
        {
            Terraria.Graphics.Effects.Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(1.2f).UseIntensity(1f)
                    .UseColor(Color.Black).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Vignette", AssetRequestMode.ImmediateLoad).Value);
            player.ManageSpecialBiomeVisuals("MoR:FogOverlay", isActive);
            player.ManageSpecialBiomeVisuals("MoR:IslandEffect", isActive);
        }
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.GetModPlayer<BuffPlayer>().island;
        }
    }
    public class SpiritRealmMusic : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/SpiritRealm");
        public override SceneEffectPriority Priority => SceneEffectPriority.Event;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.RedemptionAbility().SpiritwalkerActive;
        }
    }
}
