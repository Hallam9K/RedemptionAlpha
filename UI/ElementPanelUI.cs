using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;
using Redemption.BaseExtension;
using Terraria.ID;
using Terraria.Audio;
using ReLogic.Graphics;
using Redemption.Globals;

namespace Redemption.UI
{
    public class ElementPanelUI : UIState
    {
        public static bool Visible = false;

        public UIImage BgSprite = new(Request<Texture2D>("Redemption/UI/ElementPanelUI", ReLogic.Content.AssetRequestMode.ImmediateLoad));
        public Vector2 lastScreenSize;
        public override void OnInitialize()
        {
            lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            BgSprite.Width.Set(388, 0f);
            BgSprite.Height.Set(510, 0f);
            BgSprite.Left.Set((Main.screenWidth / 2f) + 388f / 2f, 0f);
            BgSprite.Top.Set((Main.screenHeight / 2f) + 510f / 2f, 0f);

            UIImageButton closeButton = new(Request<Texture2D>("Redemption/UI/ButtonClosePlaceholder", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            closeButton.Width.Set(22, 0f);
            closeButton.Height.Set(22, 0f);
            closeButton.Left.Set(388 - 42, 0f);
            closeButton.Top.Set(6, 0f);

            closeButton.OnLeftClick += new MouseEvent(CloseMenu);
            BgSprite.Append(closeButton);

            Append(BgSprite);
        }
        private void CloseMenu(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Visible = false;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.LocalPlayer.releaseInventory)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Visible = false;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            if (lastScreenSize != new Vector2(Main.screenWidth, Main.screenHeight))
            {
                lastScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
                BgSprite.Left.Pixels = (Main.screenWidth / 2f) - 388f / 2f;
                BgSprite.Top.Pixels = (Main.screenHeight / 2f) - 510f / 2f;
                BgSprite.Recalculate();
            }

            if (BgSprite.ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;

            base.Draw(spriteBatch);

            string titleText = Language.GetTextValue("Mods.Redemption.UI.ElementPanelUI.Title");

            string resistEntry = Language.GetTextValue("Mods.Redemption.UI.ElementPanelUI.Resistance");
            string damageEntry = Language.GetTextValue("Mods.Redemption.UI.ElementPanelUI.Damage");
            string resistValueEntry = "";
            string damageValueEntry = "";
            float[] elementResist = Main.LocalPlayer.RedemptionPlayerBuff().ElementalResistance;
            float[] elementDamage = Main.LocalPlayer.RedemptionPlayerBuff().ElementalDamage;
            for (int i = 1; i < elementResist.Length; i++)
            {
                elementResist[i] = (int)Math.Round(elementResist[i] * 100);
                elementResist[i] /= 100;

                elementDamage[i] = (int)Math.Round(elementDamage[i] * 100);
                elementDamage[i] /= 100;

                string s = i switch
                {
                    2 => ElementID.FireS,
                    3 => ElementID.WaterS,
                    4 => ElementID.IceS,
                    5 => ElementID.EarthS,
                    6 => ElementID.WindS,
                    7 => ElementID.ThunderS,
                    8 => ElementID.HolyS,
                    9 => ElementID.ShadowS,
                    10 => ElementID.NatureS,
                    11 => ElementID.PoisonS,
                    12 => ElementID.BloodS,
                    13 => ElementID.PsychicS,
                    14 => ElementID.CelestialS,
                    _ => ElementID.ArcaneS,
                };
                int resistValue = (int)(elementResist[i] * 100);
                int damageValue = (int)(elementDamage[i] * 100);

                string colorCodeR = "[c/808080:";
                string colorCodeD = "[c/808080:";
                if (resistValue < 0)
                    colorCodeR = "[c/BC7777:";
                else if (resistValue > 0)
                    colorCodeR = "[c/74B874:";

                if (damageValue < 0)
                    colorCodeD = "[c/BC7777:";
                else if (damageValue > 0)
                    colorCodeD = "[c/74B874:";

                resistEntry += "\n" + s;
                damageEntry += "\n" + s;

                resistValueEntry += "\n -";
                damageValueEntry += "\n -";

                int spaces = 0;
                int spaces2 = 0;
                if (Math.Abs(resistValue) < 100)
                    spaces++;
                if (Math.Abs(resistValue) < 10)
                    spaces++;
                if (resistValue >= 0)
                    spaces++;
                if (Math.Abs(damageValue) < 100)
                    spaces2++;
                if (Math.Abs(damageValue) < 10)
                    spaces2++;
                if (damageValue >= 0)
                    spaces2++;

                for (int k = 0; k < spaces; k++)
                    resistValueEntry += " ";
                for (int k = 0; k < spaces2; k++)
                    damageValueEntry += " ";

                resistValueEntry += colorCodeR + resistValue.ToString() + "%]";
                damageValueEntry += colorCodeD + damageValue.ToString() + "%]";
            }

            Vector2 CenterPosition;
            CenterPosition = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            int centerX = (int)CenterPosition.X;
            int centerY = (int)CenterPosition.Y;

            DynamicSpriteFont font = FontAssets.DeathText.Value;
            int titleLength = (int)font.MeasureString(titleText).X;
            Color color = new((byte)(255f * Main.mouseTextColor / 255f), (byte)(255f * Main.mouseTextColor / 255f), (byte)(255f * Main.mouseTextColor / 255f));

            Vector2 titlepos = new(centerX - (titleLength / 2), centerY - 245);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, titleText, titlepos, color, 0, Vector2.Zero, Vector2.One, 380);

            Vector2 textpos = new(centerX + 267 - 450, centerY - 165);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, resistEntry, textpos, color, 0, Vector2.Zero, Vector2.One, 380);
            Vector2 textpos2 = new(centerX + 465 - 450, centerY - 165);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, damageEntry, textpos2, color, 0, Vector2.Zero, Vector2.One, 380);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, resistValueEntry, textpos + new Vector2(90, 0), color, 0, Vector2.Zero, Vector2.One, 380);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, damageValueEntry, textpos2 + new Vector2(90, 0), color, 0, Vector2.Zero, Vector2.One, 380);
        }
    }
}