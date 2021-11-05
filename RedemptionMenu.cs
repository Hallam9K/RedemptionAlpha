using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Backgrounds;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Redemption
{
    public class RedemptionMenu : ModMenu
    {
        private const string MenuAssetPath = "Redemption/Textures/Menu";

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoScale = 0.75f;
            return true;
        }
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<WastelandSurfaceBackgroundStyle>();
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{MenuAssetPath}/Logo");

        public override string DisplayName => "Ruined Kingdom";

        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/RuinedKingdom");

        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{MenuAssetPath}/Empty");

        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{MenuAssetPath}/Empty");
    }
}