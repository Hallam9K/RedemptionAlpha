using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.Audio;
using Redemption.BaseExtension;
using ReLogic.Content;
using Terraria.GameContent;
using ReLogic.Graphics;
using Terraria.Localization;

namespace Redemption.UI
{
    public class YesNoUI : UIState
    {
        private string YesText;
        private string NoText;
        public Vector2? YesTextOffset = null;
        public Vector2? NoTextOffset = null;
        public float YesTextScale = 1;
        public float NoTextScale = 1;
        public void DisplayYesNoButtons(string yesText = "Yes", string noText = "No", Vector2? textOffset = null, Vector2? textOffset2 = null, float textScale = 1, float textScale2 = 1)
        {
            if (!Main.dedServ)
            {
                YesText = yesText;
                NoText = noText;
                YesTextOffset = textOffset;
                NoTextOffset = textOffset2;
                YesTextScale = textScale;
                NoTextScale = textScale2;
                Visible = true;
            }
        }

        private readonly UIImage YesButtonTexture = new(ModContent.Request<Texture2D>("Redemption/UI/YesButton", AssetRequestMode.ImmediateLoad));
        private readonly UIImage NoButtonTexture = new(ModContent.Request<Texture2D>("Redemption/UI/NoButton", AssetRequestMode.ImmediateLoad));
        private readonly Asset<Texture2D> Button_MouseOverTexture = ModContent.Request<Texture2D>("Redemption/UI/YesNoButton_Hover", AssetRequestMode.ImmediateLoad);
        public static bool Visible = false;

        public UIImage YesIcon;
        public UIImage NoIcon;
        public UIHoverTextImageButton YesIconHighlight;
        public UIHoverTextImageButton NoIconHighlight;
        public override void OnInitialize()
        {
            YesIcon = YesButtonTexture;
            YesIcon.Left.Set(115, 0f);
            YesIcon.Top.Set(258, 0f);
            Append(YesIcon);

            YesIconHighlight = new UIHoverTextImageButton(Button_MouseOverTexture, Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.Yes"));
            YesIconHighlight.Left.Set(-2, 0f);
            YesIconHighlight.Top.Set(-2, 0f);
            YesIconHighlight.SetVisibility(1f, 0f);
            YesIconHighlight.OnLeftClick += YesIconHighlight_OnClick;
            YesIcon.Append(YesIconHighlight);

            NoIcon = NoButtonTexture;
            NoIcon.Left.Set(267, 0f);
            NoIcon.Top.Set(258, 0f);
            Append(NoIcon);

            NoIconHighlight = new UIHoverTextImageButton(Button_MouseOverTexture, Language.GetTextValue("Mods.Redemption.GenericTerms.Choice.No"));
            NoIconHighlight.Left.Set(-2, 0f);
            NoIconHighlight.Top.Set(-2, 0f);
            NoIconHighlight.SetVisibility(1f, 0f);
            NoIconHighlight.OnLeftClick += NoIconHighlight_OnClick;
            NoIcon.Append(NoIconHighlight);

            base.OnActivate();
        }
        public override void MouseOver(UIMouseEvent evt)
        {
            YesIconHighlight.Text = YesText;
            NoIconHighlight.Text = NoText;
        }
        private void YesIconHighlight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!Main.playerInventory)
                return;

            SoundEngine.PlaySound(SoundID.Chat);
            Main.LocalPlayer.Redemption().yesChoice = true;
            Visible = false;
        }
        private void NoIconHighlight_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!Main.playerInventory)
                return;

            SoundEngine.PlaySound(SoundID.Chat);
            Main.LocalPlayer.Redemption().noChoice = true;
            Visible = false;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || !Main.playerInventory)
                return;
            base.Draw(spriteBatch);

            DynamicSpriteFont font = FontAssets.DeathText.Value;
            int textLength = (int)(font.MeasureString(YesText).X * YesTextScale);
            int textHeight = (int)(font.MeasureString(YesText).Y * YesTextScale);
            int textLength2 = (int)(font.MeasureString(NoText).X * NoTextScale);
            int textHeight2 = (int)(font.MeasureString(NoText).Y * NoTextScale);

            if (YesTextOffset == null)
                YesTextOffset = Vector2.Zero;
            if (NoTextOffset == null)
                NoTextOffset = Vector2.Zero;

            spriteBatch.DrawString(font, YesText, new Vector2(115 + 65 - (textLength / 2), 258 - 35 + (int)(textHeight * .6f)) + (Vector2)YesTextOffset, Color.White, 0, new Vector2(0, 0), YesTextScale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, NoText, new Vector2(267 + 65 - (textLength2 / 2), 258 - 35 + (int)(textHeight2 * .6f)) + (Vector2)NoTextOffset, Color.White, 0, new Vector2(0, 0), NoTextScale, SpriteEffects.None, 0);
        }
    }
}
