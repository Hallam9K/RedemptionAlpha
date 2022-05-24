using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.Sounds.Custom
{
    public static class CustomSounds
    {
		public static void UpdateLoopingSound(ref SlotId slot, SoundStyle style, float volume, float pitch = 0f, Vector2? position = null)
		{
			SoundEngine.TryGetActiveSound(slot, out var sound);

			if (volume > 0f)
			{
				if (sound == null)
				{
					slot = SoundEngine.PlaySound(style with { Volume = volume, Pitch = pitch }, position);
					return;
				}

				sound.Position = position;
				sound.Volume = volume;
			}
			else if (sound != null)
			{
				sound.Stop();

				slot = SlotId.Invalid;
			}
		}
		public static readonly SoundStyle MaskBreak = new("Redemption/Sounds/Custom/MaskBreak")
        {
            PitchVariance = 0.2f
        };
        public static readonly SoundStyle LiftLoop = new("Redemption/Sounds/Custom/ElevatorLoop");
    }
}