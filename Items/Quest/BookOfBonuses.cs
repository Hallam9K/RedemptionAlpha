using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Redemption.Items.Quest
{
    public class BookOfBonuses : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToQuestFish();
            Item.width = 42;
            Item.height = 40;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.keyState.PressingShift())
            {
                string bonusList = "";
                for (int i = 0; i < 20; i++)
                {
                    if (i == (int)RedeQuest.Bonuses.Clash)
                        continue;
                    string s = ElementID.BonusNameFromID(i);
                    string bonus;
                    if (!RedeQuest.bonusDiscovered[i])
                    {
                        bonus = "[c/808080:(Undiscovered)]";
                    }
                    else
                    {
                        bonus = ElementID.BonusDescFromID(i);
                    }

                    if (i > 0)
                        bonusList += "\n";
                    bonusList += s + ": " + bonus;
                }
                int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("ItemName"));
                if (tooltipLocation != -1)
                {
                    TooltipLine line = new(Mod, "BonusesLine", bonusList);
                    tooltips.Insert(tooltipLocation + 3, line);
                }
            }
            else
            {
                int tooltipLocation = tooltips.FindIndex(TooltipLine => TooltipLine.Name.Equals("ItemName"));
                if (tooltipLocation != -1)
                {
                    TooltipLine line = new(Mod, "HoldShift", Language.GetTextValue("Mods.Redemption.SpecialTooltips.Viewer2"))
                    {
                        OverrideColor = Color.LightGray,
                    };
                    tooltips.Insert(tooltipLocation + 3, line);
                }
            }
        }
    }
    public class BookOfBonusesInGameNotification(string bonusType, int bonusIcon) : IInGameNotification
    {
        private readonly string BonusType = bonusType;
        private readonly int BonusIcon = bonusIcon;

        public bool ShouldBeRemoved => timeLeft <= 0;

        private int timeLeft = 5 * 60;

        private readonly Asset<Texture2D> iconTexture = TextureAssets.Item[ItemType<BookOfBonuses>()];
        private Asset<Texture2D> bonusIconTexture;

        private float Scale
        {
            get
            {
                if (timeLeft < 30)
                    return MathHelper.Lerp(0f, 1f, timeLeft / 30f);
                if (timeLeft > 285)
                    return MathHelper.Lerp(1f, 0f, (timeLeft - 285) / 15f);
                return 1f;
            }
        }

        private float Opacity
        {
            get
            {
                if (Scale <= 0.5f)
                    return 0f;
                return (Scale - 0.5f) / 0.5f;
            }
        }

        public void Update()
        {
            timeLeft--;

            if (timeLeft < 0)
                timeLeft = 0;
        }

        public void DrawInGame(SpriteBatch spriteBatch, Vector2 bottomAnchorPosition)
        {
            if (Opacity <= 0f)
                return;

            bonusIconTexture ??= TextureAssets.Item[BonusIcon];

            string title = Language.GetTextValue("Mods.Redemption.UI.BookOfBonusesInGameNotification", BonusType);

            float effectiveScale = Scale * 1.1f;
            Vector2 size = (FontAssets.ItemStack.Value.MeasureString(title) + new Vector2(68f, 10f)) * effectiveScale;
            Rectangle panelSize = Utils.CenteredRectangle(bottomAnchorPosition + new Vector2(0f, (0f - size.Y) * 0.5f), size);

            // Check if the mouse is hovering over the notification.
            bool hovering = panelSize.Contains(Main.MouseScreen.ToPoint());

            Utils.DrawInvBG(spriteBatch, panelSize, new Color(163, 101, 65) * (hovering ? 0.75f : 0.5f));
            float iconScale = effectiveScale * 0.7f;
            Vector2 vector = panelSize.Right() - Vector2.UnitX * effectiveScale * (6f + iconScale * iconTexture.Width());
            spriteBatch.Draw(iconTexture.Value, vector, null, Color.White * Opacity, 0f, new Vector2(0f, iconTexture.Width() / 2f), iconScale, SpriteEffects.None, 0f);
            Utils.DrawBorderString(color: new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor / 5, Main.mouseTextColor) * Opacity, sb: spriteBatch, text: title, pos: vector - Vector2.UnitX * 10f, scale: effectiveScale * 0.9f, anchorx: 1f, anchory: 0.4f);

            if (hovering)
            {
                OnMouseOver();
            }
        }

        private void OnMouseOver()
        {
            if (PlayerInput.IgnoreMouseInterface)
            {
                return;
            }

            Main.LocalPlayer.mouseInterface = true;

            if (!Main.mouseLeft || !Main.mouseLeftRelease)
            {
                return;
            }

            Main.mouseLeftRelease = false;

            if (timeLeft > 30)
            {
                timeLeft = 30;
            }
        }

        public void PushAnchor(ref Vector2 positionAnchorBottom)
        {
            positionAnchorBottom.Y -= 50f * Opacity;
        }
    }
}