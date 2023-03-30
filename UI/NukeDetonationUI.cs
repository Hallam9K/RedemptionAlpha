using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Audio;
using Redemption.Globals;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.GameInput;

namespace Redemption.UI
{
    public class NukeDetonationUI : UIState
    {
        private readonly UIImage BgSprite = new(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_BG", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private readonly UIImage ButtonSprite = new(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_Button1", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private readonly UIImageButton SwitchSprite1 = new(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_SwitchDown", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        private readonly UIImageButton SwitchSprite2 = new(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_SwitchDown", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        public readonly UIImageButton Button = new(ModContent.Request<Texture2D>(Redemption.EMPTY_TEXTURE));

        public Vector2 lastScreenSize;

        public int ButtonState = 0;
        public bool Switch1State;
        public bool Switch2State;
        public int Timer = 0;
        public static bool Visible = false;

        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);

            BgSprite.Width.Set(608, 0f);
            BgSprite.Height.Set(332, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2f) - (332f / 2f), 0f);
            BgSprite.Left.Set((Main.screenWidth / 2f) - (608f / 2f), 0f);

            SwitchSprite1.Left.Set(216, 0f);
            SwitchSprite1.Top.Set(108, 0f);
            SwitchSprite1.Width.Set(52, 0f);
            SwitchSprite1.Height.Set(48, 0f);
            SwitchSprite1.OnLeftClick += new MouseEvent(Switch1Clicked);
            BgSprite.Append(SwitchSprite1);

            SwitchSprite2.Left.Set(216, 0f);
            SwitchSprite2.Top.Set(192, 0f);
            SwitchSprite2.Width.Set(52, 0f);
            SwitchSprite2.Height.Set(48, 0f);
            SwitchSprite2.OnLeftClick += new MouseEvent(Switch2Clicked);
            BgSprite.Append(SwitchSprite2);

            ButtonSprite.Left.Set(8, 0f);
            ButtonSprite.Top.Set(-112, 0f);
            ButtonSprite.Width.Set(204, 0f);
            ButtonSprite.Height.Set(396, 0f);
            Button.Left.Set(26, 0f);
            Button.Top.Set(104, 0f);
            Button.Width.Set(164, 0f);
            Button.Height.Set(164, 0f);
            Button.OnLeftClick += new MouseEvent(NukeButtonClicked);
            BgSprite.Append(ButtonSprite);
            BgSprite.Append(Button);

            UIImageButton closeButton = new(ModContent.Request<Texture2D>("Redemption/UI/ButtonClosePlaceholder", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.Left.Set(608 - 30, 0f);
            closeButton.Top.Set(8, 0f);

            closeButton.OnLeftClick += new MouseEvent(CloseMenu);
            //closeButton.MouseOver 
            BgSprite.Append(closeButton);

            Append(BgSprite);
        }
        private void Switch1Clicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            SwitchSprite1.SetImage(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_SwitchUp", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            Switch1State = true;
            Main.isMouseLeftConsumedByUI = true;
        }
        private void Switch2Clicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            SwitchSprite2.SetImage(ModContent.Request<Texture2D>("Redemption/UI/NukeDetonationUI_SwitchUp", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            Switch2State = true;
            Main.isMouseLeftConsumedByUI = true;
        }
        private void NukeButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (Switch1State && Switch2State)
            {
                bool fail = false;
                for (int x = -44; x <= 44; x++)
                {
                    for (int y = -44; y <= 44; y++)
                    {
                        Point tileToWarhead = RedeWorld.nukeGroundZero.ToTileCoordinates();
                        int type = Main.tile[tileToWarhead.X + x, tileToWarhead.Y + y].TileType;
                        if (Main.tile[tileToWarhead.X + x, tileToWarhead.Y + y] != null && Main.tile[tileToWarhead.X + x, tileToWarhead.Y + y].HasTile)
                        {
                            if (Main.tileDungeon[type] || type == 88 || type == 21 || type == 26 || type == 107 || type == 108 || type == 111 || type == 226 || type == 237 || type == 221 || type == 222 || type == 223 || type == 211)
                                fail = true; 
                            if (!TileLoader.CanExplode(tileToWarhead.X + x, tileToWarhead.Y + y))
                                fail = true;
                        }
                    }
                }
                if (fail)
                {
                    string status = "The bomb is too close to unexplodable tiles";
                    if (Main.netMode == NetmodeID.Server)
                        ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.White);
                    else if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(status), Color.White);
                    return;
                }
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
            }
        }
        private void CloseMenu(UIMouseEvent evt, UIElement listeningElement)
        {
            if (ButtonState != 2)
            {
                ButtonState = 0;

                Visible = false;
            }

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
                Main.LocalPlayer.mouseInterface = true;

            if (ButtonState != 2 && !Main.LocalPlayer.releaseInventory)
                Visible = false;

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

            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                BgSprite.Left.Pixels = (Main.screenWidth / 2f) - (608f / 2f);
                BgSprite.Top.Pixels = (Main.screenHeight / 2f) - (332f / 2f);
                BgSprite.Recalculate();
            }

            base.Draw(spriteBatch);
        }
    }
}
