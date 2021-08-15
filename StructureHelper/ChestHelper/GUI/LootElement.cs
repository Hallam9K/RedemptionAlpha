using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Redemption.StructureHelper.ChestHelper.GUI
{
    class LootElement : UIElement
    {
        Loot loot;
        UIImageButton removeButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/Cross"));

        NumberSetter min;
        NumberSetter max;
        NumberSetter weight;

        UIImageButton upButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/Up"));
        UIImageButton downButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/Down"));

        public LootElement(Loot loot, bool hasWeight)
        {
            this.loot = loot;

            Width.Set(350, 0);
            Height.Set(36, 0);
            Left.Set(50, 0);

            removeButton.Left.Set(-36, 1);
            removeButton.Width.Set(32, 0);
            removeButton.Height.Set(32, 0);
            removeButton.OnClick += Remove;
            Append(removeButton);

            min = new NumberSetter(loot.min, "Minimum", 80);
            Append(min);

            max = new NumberSetter(loot.max, "Maximum", 120);
            Append(max);

            if (hasWeight)
            {
                weight = new NumberSetter(loot.weight, "Weight", 160);
                Append(weight);
            }
			else
			{
                upButton.Left.Set(8, 0);
                upButton.Top.Set(6, 0);
                upButton.Width.Set(12, 0);
                upButton.Height.Set(8, 0);
                upButton.SetVisibility(1, 0.8f);
                upButton.OnClick += MoveUp;
                Append(upButton);

                downButton.Left.Set(8, 0);
                downButton.Top.Set(16, 0);
                downButton.Width.Set(12, 0);
                downButton.Height.Set(8, 0);
                downButton.SetVisibility(1, 0.8f);
                downButton.OnClick += MoveDown;
                Append(downButton);
            }
        }

        private void MoveDown(UIMouseEvent evt, UIElement listeningElement)
        {
            var list = Parent.Parent as UIList;
            int i = list._items.IndexOf(this);

            if (i < list.Count - 1)
            {
                var temp = list._items[i];
                list._items[i] = list._items[i + 1];
                list._items[i + 1] = temp;
            }
        }

        private void MoveUp(UIMouseEvent evt, UIElement listeningElement)
        {
            var list = Parent.Parent as UIList;
            int i = list._items.IndexOf(this);

            if (i >= 1)
            {
                var temp = list._items[i];
                list._items[i] = list._items[i - 1];
                list._items[i - 1] = temp;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            var target = new Rectangle((int)pos.X, (int)pos.Y, (int)GetDimensions().Width, 32);

            var color = Color.White;

            if (Parent.Parent.Parent is ChestRuleElement)
                color = (Parent.Parent.Parent as ChestRuleElement).color;

            if (removeButton.IsMouseHovering)
                Main.hoverItemName = "Remove item";

            ManualGeneratorMenu.DrawBox(spriteBatch, target, color);

            int xOff = 0;
            if (weight is null)
                xOff += 16;

            spriteBatch.Draw(Helper.GetItemTexture(loot.LootItem), new Rectangle((int)pos.X + 8 + xOff, (int)pos.Y + 8, 16, 16), Color.White);

            string name = loot.LootItem.Name.Length > 25 ? loot.LootItem.Name.Substring(0, 23) + "..." : loot.LootItem.Name;
            Utils.DrawBorderString(spriteBatch, name, pos + new Vector2(28 + xOff, 10), Color.White, 0.7f);

            if (min.Value > max.Value)
                min.Value = max.Value;

            if (max.Value < min.Value)
                max.Value = min.Value;

            loot.min = min.Value;
            loot.max = max.Value;

            if (weight != null)
                loot.weight = weight.Value;

            base.Draw(spriteBatch);
        }

        private void Remove(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(Parent.Parent.Parent is ChestRuleElement)) return;

            ChestRuleElement parent = Parent.Parent.Parent as ChestRuleElement;
            parent.RemoveItem(loot, this);
        }
    }
}
