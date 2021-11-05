using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals.Player;
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
            return RedeSystem.Silence || player.GetModPlayer<Radiation>().irradiatedEffect == 2;
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
            return player.GetModPlayer<Radiation>().irradiatedEffect == 3;
        }
    }
    public class Rad2Music : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Rad2");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            return player.GetModPlayer<Radiation>().irradiatedEffect >= 4;
        }
    }
}