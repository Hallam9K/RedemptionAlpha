using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Redemption
{
    public class RedeConfigClient : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static RedeConfigClient Instance;

        [Label("Disable Zelda-Styled Boss Titles")]
        [Tooltip("Disables the Legend of Zelda-style boss introduction text for bosses.")]
        public bool NoBossIntroText;

        [Label("Disable Camera Lock")]
        [Tooltip("Disables the locked camera during specific boss fights")]
        public bool CameraLockDisable;

        [Label("Disable Elements")]
        [Tooltip("Disables elemental resistances and damage")]
        public bool ElementDisable;
    }
}