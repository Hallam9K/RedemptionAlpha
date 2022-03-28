using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.Sounds.Custom
{
    public class MaskBreak : ModSound
    {
        public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan)
        {
            soundInstance.Volume = volume * 1f;
            soundInstance.Pan = pan;
            soundInstance.Pitch = Main.rand.Next(-5, 6) * 0.02f;
            return soundInstance;
        }
    }
}
