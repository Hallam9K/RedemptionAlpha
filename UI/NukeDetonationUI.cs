using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Audio;
using Redemption.Globals;
using Redemption.Base;
using Terraria.Localization;
using Terraria.Chat;

namespace Redemption.UI
{
    public class NukeDetonationUI : UIState
    {
        private readonly UIImage BgSprite = new(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_BG", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private readonly UIImage ButtonSprite = new(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_Button1", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        public readonly UIImageButton Button = new(ModContent.Request<Texture2D>(Redemption.EMPTY_TEXTURE));

        public int ButtonState = 0;
        public int Timer = 0;
        public static bool Visible = false;

        public override void OnInitialize()
        {
            BgSprite.Width.Set(608, 0f);
            BgSprite.Height.Set(332, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2) - 332 / 2, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2) - 608 / 2, 0f);
            BgSprite.VAlign = BgSprite.HAlign = 0.5f;

            ButtonSprite.Left.Set(8, 0f);
            ButtonSprite.Top.Set(-112, 0f);
            ButtonSprite.Width.Set(204, 0f);
            ButtonSprite.Height.Set(396, 0f);
            Button.Left.Set(26, 0f);
            Button.Top.Set(104, 0f);
            Button.Width.Set(164, 0f);
            Button.Height.Set(164, 0f);
            Button.OnClick += new MouseEvent(NukeButtonClicked);
            BgSprite.Append(ButtonSprite);
            BgSprite.Append(Button);

            UIImageButton closeButton = new(ModContent.Request<Texture2D>("Redemption/UI/ButtonClosePlaceholder", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.Left.Set(608 - 30, 0f);
            closeButton.Top.Set(8, 0f);

            closeButton.OnClick += new MouseEvent(CloseMenu);
            //closeButton.MouseOver 
            BgSprite.Append(closeButton);

            Append(BgSprite);
        }
        private void NukeButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (ButtonState < 2 && Vector2.Distance(RedeWorld.nukeGroundZero, new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16)) > (Main.maxTilesX / 8 * 16) && RedeWorld.nukeGroundZero.Y < (Main.worldSurface * 16f))
            {
                ++ButtonState;
            }
            else if (ButtonState < 2)
            {
                string status = "The bomb must be activated on the surface and in the far reaches of the world";
                if (Main.netMode == NetmodeID.Server)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.White);
                else if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(status), Color.White);
            }
            Main.isMouseLeftConsumedByUI = true;
        }
        private void CloseMenu(UIMouseEvent evt, UIElement listeningElement)
        {
            if (ButtonState != 2)
            {
                ButtonState = 0;

                Visible = false;
            }

        }
        public override void MouseOver(UIMouseEvent evt)
        {
            Main.isMouseLeftConsumedByUI = true;
        }
        public override void Update(GameTime gameTime)
        {
            if (!Visible)
                ButtonState = 0;

            switch (ButtonState)
            {
                case 0:
                    ButtonSprite.SetImage(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_Button1", ReLogic.Content.AssetRequestMode.ImmediateLoad));
                    break;
                case 1:
                    ButtonSprite.SetImage(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_Button2", ReLogic.Content.AssetRequestMode.ImmediateLoad));
                    break;
                case 2:
                    ButtonSprite.SetImage(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_Button3", ReLogic.Content.AssetRequestMode.ImmediateLoad));
                    break;
            }

            if (ButtonState == 2)
            {
                ++Timer;

                if (Timer > 60)
                {
                    ButtonState = 3;
                    Timer = 0;
                    RedeWorld.nukeCountdownActive = true;
                    Visible = false;
                }
            }
            else
                Timer = 0;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            base.Draw(spriteBatch);
        }
    }
}
