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
            SoundEffectInstance instance = Sound.Value.CreateInstance();
            instance.Volume = volume * 1f;
            instance.Pan = pan;
            instance.Pitch = Main.rand.Next(-5, 6) * 0.02f;
            instance.Play();
            SoundInstanceGarbageCollector.Track(instance);
            return instance;
        }
    }
}