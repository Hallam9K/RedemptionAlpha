using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class AMemoryUIState : UIState
    {
        public UIImage BgSprite = new(ModContent.Request<Texture2D>("Redemption/UI/AMemory", ReLogic.Content.AssetRequestMode.ImmediateLoad));

        public static bool Visible = false;

        public override void OnInitialize()
        {
            BgSprite.Width.Set(426, 0f);
            BgSprite.Height.Set(438, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2) - 332 / 2, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2) - 608 / 2, 0f);
            BgSprite.VAlign = BgSprite.HAlign = 0.5f;

            UIImageButton closeButton = new(ModContent.Request<Texture2D>("Redemption/UI/ButtonClosePlaceholder", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.Left.Set(426 - 30, 0f);
            closeButton.Top.Set(8, 0f);

            closeButton.OnClick += new MouseEvent(CloseMenu);
            BgSprite.Append(closeButton);

            Append(BgSprite);
        }
        private void CloseMenu(UIMouseEvent evt, UIElement listeningElement)
        {
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

            base.Draw(spriteBatch);
        }
    }
}
