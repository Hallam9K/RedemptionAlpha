using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Audio;
using SubworldLibrary;
using Redemption.WorldGeneration.Space;
using Terraria.Utilities;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Redemption.UI
{
    public class CyberTeleporterUI : UIState
    {
        private readonly UIImage BgSprite = new(ModContent.Request<Texture2D>("Redemption/UI/CyberTeleporterUI", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private readonly UIImageButton Button = new(ModContent.Request<Texture2D>("Redemption/UI/CyberTeleporterUI", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private string Text;
        public void DisplayCyberTeleporterUI(string text)
        {
            Text = text;
            Visible = true;
        }
        public Vector2 lastScreenSize;

        public static bool Visible = false;

        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            BgSprite.Width.Set(320, 0f);
            BgSprite.Height.Set(132, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2f) + 320f / 2f, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2f) + 132f / 2f, 0f);

            Button.Left.Set(0, 0f);
            Button.Top.Set(0, 0f);
            Button.Width.Set(320, 0f);
            Button.Height.Set(132, 0f);
            Button.OnClick += new MouseEvent(ButtonClicked);
            BgSprite.Append(Button);

            UIImageButton closeButton = new(ModContent.Request<Texture2D>("Redemption/UI/ButtonClosePlaceholder", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.Left.Set(320f - 30, 0f);
            closeButton.Top.Set(8, 0f);

            closeButton.OnClick += new MouseEvent(CloseMenu);
            BgSprite.Append(closeButton);

            Append(BgSprite);
        }
        private void ButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.isMouseLeftConsumedByUI = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
            SoundEngine.PlaySound(SoundID.Item163);
            Visible = false;
            if (SubworldSystem.IsActive<SpaceSub>())
            {
                SubworldSystem.Exit();
                return;
            }
            else if (!SubworldSystem.AnyActive<Redemption>())
            {
                Main.rand = new UnifiedRandom();
                SubworldSystem.Enter<SpaceSub>();
            }
        }
        private void CloseMenu(UIMouseEvent evt, UIElement listeningElement)
        {
            Visible = false;
        }
        public override void Update(GameTime gameTime)
        {
            Main.LocalPlayer.mouseInterface = true;
            if (!Main.LocalPlayer.releaseInventory)
                Visible = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                BgSprite.Left.Pixels = (Main.screenWidth / 2f) - 320f / 2f;
                BgSprite.Top.Pixels = (Main.screenHeight / 2f) - 132f / 2f;
                BgSprite.Recalculate();
            }

            base.Draw(spriteBatch);

            Vector2 CenterPosition;
            CenterPosition = new Vector2(BgSprite.Left.Pixels + 150f, BgSprite.Top.Pixels + 66f);
            int centerX = (int)CenterPosition.X;
            int centerY = (int)CenterPosition.Y;
            int textLength = (int)FontAssets.MouseText.Value.MeasureString(Text).X;
            int textHeight = (int)FontAssets.MouseText.Value.MeasureString(Text).Y;
            Vector2 textpos = new(centerX - textLength, centerY - textHeight);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, Text, textpos, Color.LightCyan, 0, Vector2.Zero, Vector2.One);
        }
    }
}
