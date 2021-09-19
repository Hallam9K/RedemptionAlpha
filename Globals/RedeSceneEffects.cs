using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Redemption
{
    public class SilenceEffect : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/silence");
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override bool IsSceneEffectActive(Player player)
        {
            return RedeSystem.Silence;
        }
    }
}