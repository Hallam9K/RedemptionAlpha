using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.Sounds.Custom
{
    public static class CustomSounds
    {
        public static readonly SoundStyle MaskBreak = new("Redemption/Sounds/Custom/MaskBreak")
        {
            PitchVariance = 0.2f
        };
    }
}