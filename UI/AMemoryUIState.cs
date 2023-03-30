using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.UI
{
    public class AMemoryUIState : UIState
    {
        public UIImage BgSprite = new(ModContent.Request<Texture2D>("Redemption/UI/AMemory", ReLogic.Content.AssetRequestMode.ImmediateLoad));

        public static bool Visible = false;
        public Vector2 lastScreenSize;

        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            BgSprite.Width.Set(426, 0f);
            BgSprite.Height.Set(438, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2f) + 426f / 2f, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2f) + 438f / 2f, 0f);

            UIImageButton closeButton = new(ModContent.Request<Texture2D>("Redemption/UI/ButtonClosePlaceholder", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.Left.Set(426 - 30, 0f);
            closeButton.Top.Set(8, 0f);

            closeButton.OnLeftClick += new MouseEvent(CloseMenu);
            BgSprite.Append(closeButton);

            Append(BgSprite);
        }
        private void CloseMenu(UIMouseEvent evt, UIElement listeningElement)
        {
            Visible = false;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
                Main.LocalPlayer.mouseInterface = true;

            if (!Main.LocalPlayer.releaseInventory)
                Visible = false;
        }
        public override void MouseOver(UIMouseEvent evt)
        {
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if(lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                BgSprite.Left.Pixels = (Main.screenWidth / 2f) - 426f / 2f;
                BgSprite.Top.Pixels = (Main.screenHeight / 2f) - 438f / 2f;
                BgSprite.Recalculate();
            }

            base.Draw(spriteBatch);

            //spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/UI/AMemory").Value, new Vector2(BgSprite.Left.Pixels, BgSprite.Top.Pixels), Color.White);

        }
    }
}
