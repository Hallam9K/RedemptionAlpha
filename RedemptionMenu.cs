using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Redemption.Backgrounds;
using ReLogic.Content;
using Terraria.ModLoader;
using System;

namespace Redemption
{
    public class RedemptionMenu : ModMenu
    {
        private const string MenuAssetPath = "Redemption/Textures/Menu";

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoDrawCenter -= new Vector2(36, 0);
            logoScale = 1f;
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
    public class RedemptionMenu2 : ModMenu
    {
        private const string MenuAssetPath = "Redemption/Textures/Menu";
        float sineAcc = 0f;
        float sineAccY = 0f;
        float sineTotal = 0f;
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoDrawCenter -= new Vector2(36, 0);
            logoScale = 1f;

            Texture2D texture = ModContent.Request<Texture2D>("Redemption/Textures/Menu/EpidotraMap").Value;
            Vector2 origin = new((texture.Width / 2), (texture.Height / 2));

            float x = ((float)Math.Sin(sineAcc) * 600f) - ((float)Math.Sin(sineTotal) * 800f);
            float y = ((float)Math.Sin(sineAccY) * -500f) + ((float)Math.Sin(sineTotal) * 300f);
            Vector2 pos = logoDrawCenter - new Vector2(-180, -340) + new Vector2(x, y);
            pos.X = MathHelper.Clamp(pos.X, -500, 2500);
            spriteBatch.Draw(texture, pos, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);

            return true;
        }
        public override void Update(bool isOnTitleScreen)
        {
            Main.dayTime = true;
            Main.time = 40000;
            sineAcc += 0.0004f;
            sineAccY += 0.0002f;
            sineTotal += 0.0005f;
        }

        public override void OnSelected()
        {
            float randVal = Main.rand.NextFloat(0f, 10f);
            sineAcc = randVal * 4;
            sineAccY = randVal * 2;
            sineTotal = randVal * 5;
        }
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{MenuAssetPath}/Logo");

        public override string DisplayName => "Epidotra Map";

        public override int Music => MusicLoader.GetMusicSlot("Redemption/Sounds/Music/Epidotra");

        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>($"{MenuAssetPath}/Empty");

        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>($"{MenuAssetPath}/Empty");
    }
}