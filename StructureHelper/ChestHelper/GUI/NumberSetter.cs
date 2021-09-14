using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Redemption.StructureHelper.ChestHelper.GUI
{
    class NumberSetter : UIElement
    {
        public int Value;
        string Text;
        string Suffix;

        UIImageButton IncrementButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/Up"));
        UIImageButton DecrementButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/Down"));

        public NumberSetter(int value, string text, int xOff, string suffix = "")
        {
            Value = value;
            Text = text;
            Suffix = suffix;

            Width.Set(32, 0);
            Height.Set(32, 0);
            Left.Set(-xOff, 1);

            IncrementButton.Width.Set(12, 0);
            IncrementButton.Height.Set(8, 0);
            IncrementButton.Top.Set(7, 0);
            IncrementButton.OnClick += (n, w) => Value++;
            Append(IncrementButton);

            DecrementButton.Width.Set(12, 0);
            DecrementButton.Height.Set(8, 0);
            DecrementButton.Top.Set(15, 0);
            DecrementButton.OnClick += (n, w) => Value--;
            Append(DecrementButton);
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            Value += evt.ScrollWheelValue > 0 ? 1 : -1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            var target = new Rectangle((int)pos.X + 14, (int)pos.Y + 8, 20, 16);

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, target, Color.Black * 0.5f);
            Utils.DrawBorderString(spriteBatch, Value.ToString() + Suffix, target.Center(), Color.White, 0.7f, 0.5f, 0.4f);

            if (Value < 1)
                Value = 1;

            if (IsMouseHovering)
                Main.hoverItemName = Text;

            base.Draw(spriteBatch);
        }
    }
}
