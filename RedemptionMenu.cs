using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Redemption.Backgrounds;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Redemption
{
    public class RedemptionMenu : ModMenu
    {
        private const string MenuAssetPath = "Redemption/Textures/Menu";

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoDrawCenter -= new Vector2(36, 0);
            logoScale = 0.75f;
            return true;
        }
        public override void Update(bool isOnTitleScreen)
        {
            Main.dayTime = true;
            Main.time = 40000;
        }

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<RuinedKingdomSurfaceBgStyle_Menu>();
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{MenuAssetPath}/Logo");

        public override string DisplayName => "Ruined Kingdom";

        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/RuinedKingdom");

        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{MenuAssetPath}/Empty");

        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{MenuAssetPath}/Empty");
    }
}