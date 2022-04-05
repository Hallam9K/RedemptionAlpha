using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Redemption.Sounds
{
    public class SoundLooper
    {
        private SoundEffectInstance _instance;
        private Mod _mod;

        private bool _playing;
        private float _destinationVolume;
        private float _myVolume;

        public SoundLooper(Mod mod, string name, SoundType type = SoundType.Ambient)
        {
            _mod = mod;

            int style = SoundLoader.GetSoundSlot(mod, name);
            _mod.Logger.Debug($"Sound looper {name} being created. Style: {style}");
            if (style < 0 || style >= SoundLoader.SoundCount)
            {
                _mod.Logger.Warn($"Couldn't create SoundLooper object properly! style: {style} name: {name} type: {type}");
                return;
            }

            try
            {
                _instance = (SoundEffectInstance)((Array)typeof(SoundLoader).GetField("customSoundInstances", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null)).GetValue(style);
            }
            catch (Exception e)
            {
                _mod.Logger.Error($"Couldn't create SoundLooper object properly! Exception: {e}");
                return;
            }
        }

        public void Update()
        {
            if (!_playing || _instance == null) return;

            if (_instance.State == SoundState.Stopped)
            {
                _instance.Play();
            }

            if (Main.gameMenu)
            {
                Halt();
                return;
            }

            float speed = 0.01f;
            if (_myVolume < _destinationVolume)
            {
                _myVolume += speed;
                if (_myVolume > _destinationVolume) _myVolume = _destinationVolume;
            }
            else if (_myVolume > _destinationVolume)
            {
                _myVolume -= speed;
                if (_myVolume < _destinationVolume) _myVolume = _destinationVolume;
            }

            _instance.Volume = _myVolume;
            if (_myVolume <= 0.001f)
            {
                _instance.Stop();
                _playing = false;
            }
        }

        public void SetVolume(float volume)
        {
            _destinationVolume = _myVolume = volume;
        }

        public void SetTo(float volume = 1f, float pitch = 0f, float pan = 0f)
		{
			if (_instance == null)
				return;

			if (!_playing)
            {
                _playing = true;

                _instance.Pitch = pitch;
                _instance.Pan = pan;
                _instance.Play();
            }
            _destinationVolume = volume;
        }

        /// <summary>
        /// Begin's the sound's slowdown.
        /// </summary>
        public void Stop()
        {
            _destinationVolume = 0f;
        }

        /// <summary>
        /// Instantly pause the sound.
        /// </summary>
        public void Halt()
        {
			if (_instance == null)
				return;

            _instance.Stop();
            _playing = false;
        }
    }
}
