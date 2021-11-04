using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class SilenceEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/silence");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return RedeSystem.Silence;
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
}