using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Redemption
{
    public class RedeConfigClient : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static RedeConfigClient Instance;

        [Range(0, 180)]
        [DefaultValue(0)]
        [Slider]
        [Label("Extra Dialogue Wait Time")]
        [Tooltip("Increases duration of dialogue text persisting before moving on, increase if you're a slow reader\n" +
            "Value is in frames (60 = 1 extra second, caps at 180 extra frames/3 extra seconds)")]
        public int DialogueWaitTime;

        [Range(-2, 2)]
        [DefaultValue(0)]
        [Slider]
        [DrawTicks]
        [Label("Dialogue Text Speed")]
        [Tooltip("Only recommend changing if the dialogue speed is too slow or fast normally.\n" +
            "This is how many frames less for each letter to appear, higher = faster.\n" +
            "The faster you make it, the harder it is to read before finishing, so also adjust Extra Dialogue Wait Time")]
        public int DialogueSpeed;

        [Label("Disable Zelda-Styled Boss Titles")]
        [Tooltip("Disables the Legend of Zelda-style boss introduction text for bosses.")]
        public bool NoBossIntroText;

        [Label("Disable Camera Lock")]
        [Tooltip("Disables the locked camera during cutscenes, along with the invincibility and vignette effect\n" +
            "Not recommended, at least on a first playthrough, as it can ruin the feel of things.")]
        public bool CameraLockDisable;

        [Label("Disable Elements")]
        [Tooltip("Disables elemental resistances and damage")]
        public bool ElementDisable;

        [Label("No Patient Zero Build-Up")]
        [Tooltip("Makes the boss begin the fight instantly, mainly for no-hitters. (This will cause the boss's pulse effect to not sync well with the music)")]
        public bool NoPZBuildUp;
    }
}