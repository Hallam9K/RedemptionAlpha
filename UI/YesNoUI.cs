using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Audio;
using Redemption.BaseExtension;
using Terraria.GameInput;

namespace Redemption.UI
{
    public class YesNoUI : UIState
    {
        private readonly UIImage BgSprite = new(ModContent.Request<Texture2D>("Redemption/UI/YesButton", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private readonly UIImageButton Button = new(ModContent.Request<Texture2D>("Redemption/UI/YesButton", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private readonly UIImage BgSprite2 = new(ModContent.Request<Texture2D>("Redemption/UI/NoButton", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private readonly UIImageButton Button2 = new(ModContent.Request<Texture2D>("Redemption/UI/NoButton", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        public Vector2 lastScreenSize;

        public static bool Visible = false;
        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            BgSprite.Width.Set(130, 0f);
            BgSprite.Height.Set(70, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2f) - 200 + 130f / 2f, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2f) + 70f / 2f, 0f);

            Button.Left.Set(0, 0f);
            Button.Top.Set(0, 0f);
            Button.Width.Set(130, 0f);
            Button.Height.Set(70, 0f);
            Button.OnLeftClick += new MouseEvent(YesClicked);
            BgSprite.Append(Button);

            Append(BgSprite);

            BgSprite2.Width.Set(130, 0f);
            BgSprite2.Height.Set(70, 0f);
            BgSprite2.Left.Set((Main.screenWidth / 2f) + 200 + 130f / 2f, 0f);
            BgSprite2.Top.Set((Main.screenHeight / 2f) + 70f / 2f, 0f);

            Button2.Left.Set(0, 0f);
            Button2.Top.Set(0, 0f);
            Button2.Width.Set(130, 0f);
            Button2.Height.Set(70, 0f);
            Button2.OnLeftClick += new MouseEvent(NoClicked);
            BgSprite2.Append(Button2);

            Append(BgSprite2);
        }
        private void YesClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.isMouseLeftConsumedByUI = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
            Main.LocalPlayer.Redemption().yesChoice = true;
            Visible = false;
        }
        private void NoClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.isMouseLeftConsumedByUI = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
            Main.LocalPlayer.Redemption().noChoice = true;
            Visible = false;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
                Main.LocalPlayer.mouseInterface = true;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                BgSprite.Left.Pixels = (Main.screenWidth / 2f) - 200 - 130f / 2f;
                BgSprite.Top.Pixels = (Main.screenHeight / 2f) - 70f / 2f;
                BgSprite.Recalculate();
                BgSprite2.Left.Pixels = (Main.screenWidth / 2f) + 200 - 130f / 2f;
                BgSprite2.Top.Pixels = (Main.screenHeight / 2f) - 70f / 2f;
                BgSprite2.Recalculate();
            }

            base.Draw(spriteBatch);
        }
    }
}
