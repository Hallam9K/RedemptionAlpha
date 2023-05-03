using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Redemption.UI
{
    public class DatalogUIState : UIState
    {
        public static bool Visible = false;
        private string Text;
        public Vector2 lastScreenSize;
        public void DisplayDatalogText(string text)
        {
            Text = text;
            Visible = true;
        }
        public UIImage BgSprite = new(ModContent.Request<Texture2D>("Redemption/UI/DatalogUI", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            BgSprite.Width.Set(1030, 0f);
            BgSprite.Height.Set(260, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2f) + 1030f / 2f, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2f) + 260f / 2f, 0f);

            UIImageButton closeButton = new(ModContent.Request<Texture2D>("Redemption/UI/ButtonClosePlaceholder", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.Left.Set(1030 - 30, 0f);
            closeButton.Top.Set(8, 0f);

            closeButton.OnClick += new MouseEvent(CloseMenu);
            BgSprite.Append(closeButton);

            Append(BgSprite);
        }
        private void CloseMenu(UIMouseEvent evt, UIElement listeningElement)
        {
            Visible = false;
        }
        public override void Update(GameTime gameTime)
        {
            if (!Main.LocalPlayer.releaseInventory)
                Visible = false;
        }
        public override void MouseOver(UIMouseEvent evt)
        {
            Main.isMouseLeftConsumedByUI = true;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                BgSprite.Left.Pixels = (Main.screenWidth / 2f) - 1030f / 2f;
                BgSprite.Top.Pixels = (Main.screenHeight / 2f) - 260f / 2f;
                BgSprite.Recalculate();
            }

            base.Draw(spriteBatch);

            Vector2 CenterPosition;
            CenterPosition = new Vector2(BgSprite.Left.Pixels + 515f, BgSprite.Top.Pixels + 130f);
            int centerX = (int)CenterPosition.X;
            int centerY = (int)CenterPosition.Y;
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(Text).X;
            int textHeight = (int)FontAssets.MouseText.Value.MeasureString(Text).Y;
            Vector2 textpos = new(centerX - (textLength / 2f), centerY - (textHeight / 2f));
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, Text, textpos, Color.LightCyan, 0, Vector2.Zero, Vector2.One);
        }
    }
}
