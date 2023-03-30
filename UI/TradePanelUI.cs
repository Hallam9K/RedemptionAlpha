using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace Redemption.UI
{
    public class TradePanelUI : UIPanel
    {
        private Item inputItem;
        private Item outputItem;
        private int inputStack;
        private int outputStack;
        public TradePanelUI(Item inputItem, Item outputItem, int inputStack = 1, int outputStack = 1)
        {
            this.inputItem = inputItem;
            this.outputItem = outputItem;
            this.inputStack = inputStack;
            this.outputStack = outputStack;

            SetPadding(0);
            PaddingLeft = 10;
            PaddingRight = 10;

            Width.Set(-14, 1);
            Height.Set(32, 0);
            Left.Set(5, 0);

            OnMouseOver += MouseOver;
            OnMouseOut += MouseOut;

            BorderColor = new Color(89, 116, 213);

            Append(new UIItemIcon(inputItem, false)
            {
                IgnoresMouseInteraction = true,
                HAlign = 0,
                Left = new StyleDimension(4, 0)
            });
            Append(new UIText(inputStack.ToString())
            {
                HAlign = 0.25f,
                Top = new StyleDimension(8, 0),
                Left = new StyleDimension(4, 0)
            });

            Append(new UIArrow(-MathHelper.PiOver2)
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            });

            Append(new UIItemIcon(outputItem, false)
            {
                IgnoresMouseInteraction = true,
                HAlign = 1,
                Left = new StyleDimension(-4, 0)
            });
            Append(new UIText(outputStack.ToString())
            {
                HAlign = 0.75f,
                Top = new StyleDimension(8, 0),
                Left = new StyleDimension(-4, 0)
            });
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (IsMouseHovering)
            {
                Main.HoverItem = Main.mouseX <= GetDimensions().Center().X ? inputItem : outputItem;
                Main.instance.MouseText("");
                Main.mouseText = true;
            }
        }

        private void MouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.LocalPlayer.mouseInterface = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
            BorderColor = Colors.FancyUIFatButtonMouseOver;
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            int trade = Main.LocalPlayer.FindItem(inputItem.type);
            if (trade >= 0 && Main.LocalPlayer.inventory[trade].stack >= inputStack)
            {
                Main.LocalPlayer.inventory[trade].stack -= inputStack;
                if (Main.LocalPlayer.inventory[trade].stack <= 0)
                    Main.LocalPlayer.inventory[trade] = new Item();

                Main.LocalPlayer.QuickSpawnItem(new EntitySource_Misc("Trade"), outputItem, outputStack);
            }
        }

        private void MouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            BorderColor = new Color(89, 116, 213, 255);
        }
    }
    public class UIArrow : UIElement
    {
        public float rotation;

        public UIArrow(float rotation)
        {
            this.rotation = rotation;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.GolfBallArrow.Value, GetDimensions().Position(), new Rectangle(0, 0, 20, 32), Color.White, rotation, new Vector2(10, 16), 1, SpriteEffects.None, 0);
            spriteBatch.Draw(TextureAssets.GolfBallArrow.Value, GetDimensions().Position(), new Rectangle(20, 0, 20, 32), Color.Black, rotation, new Vector2(10, 16), 1, SpriteEffects.None, 0);
        }
    }
}