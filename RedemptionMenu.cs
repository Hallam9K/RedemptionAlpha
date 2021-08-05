using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;

namespace Redemption
{
    public class RedemptionMenu : ModMenu
    {
        private const string menuAssetPath = "Redemption/ExtraTextures/Menu";

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoScale = 0.75f;
            return true;
        }
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{menuAssetPath}/Logo");

        public override string DisplayName => "Ruined Kingdom";

        //public override int Music => Mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Redemption/Sounds/Music/RuinedKingdom");

        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/Empty");
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{menuAssetPath}/Empty");

    }
}