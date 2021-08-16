using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Redemption.StructureHelper.ChestHelper.GUI
{
    class ChestRuleElement : UIElement
    {
        internal ChestRule rule;
        internal Color color = Color.White;
        internal float storedHeight = 0;

        internal UIList lootElements = new UIList();
        UIImageButton removeButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/Cross"));

        UIImageButton upButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/UpLarge"));
        UIImageButton downButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/DownLarge"));
        UIImageButton hideButton = new UIImageButton(Request<Texture2D>("Redemption/StructureHelper/Eye"));

        public ChestRuleElement(ChestRule rule)
        {
            this.rule = rule;

            Width.Set(400, 0);
            Height.Set(36, 0);

            lootElements.Left.Set(0, 0);
            lootElements.Top.Set(40, 0);
            lootElements.Width.Set(400, 0);
            lootElements.Height.Set(0, 0);
            Append(lootElements);

            removeButton.Left.Set(-36, 1);
            removeButton.Width.Set(32, 0);
            removeButton.Height.Set(32, 0);
            removeButton.OnClick += Remove;
            Append(removeButton);

            upButton.Left.Set(4, 0);
            upButton.Top.Set(-4, 0);
            upButton.Width.Set(24, 0);
            upButton.Height.Set(18, 0);
            upButton.SetVisibility(1, 0.8f);
            upButton.OnClick += MoveUp;
            Append(upButton);

            downButton.Left.Set(4, 0);
            downButton.Top.Set(18, 0);
            downButton.Width.Set(24, 0);
            downButton.Height.Set(18, 0);
            downButton.SetVisibility(1, 0.8f);
            downButton.OnClick += MoveDown;
            Append(downButton);

            hideButton.Left.Set(-56, 1);
            hideButton.Top.Set(10, 0);
            hideButton.Width.Set(18, 0);
            hideButton.Height.Set(12, 0);
            hideButton.SetVisibility(1, 0.5f);
            hideButton.OnClick += Hide;
            Append(hideButton);

            foreach (Loot loot in rule.pool)
                AddItem(loot);
        }

		private void Hide(UIMouseEvent evt, UIElement listeningElement)
		{
            if (storedHeight == 0)
            {
                hideButton.SetImage(Request<Texture2D>("Redemption/StructureHelper/EyeClosed"));
                storedHeight = GetDimensions().Height;
                Height.Set(36, 0);
            }
			else
			{
                hideButton.SetImage(Request<Texture2D>("Redemption/StructureHelper/Eye"));
                Height.Set(storedHeight, 0);
                storedHeight = 0;
            }
		}

		private void MoveDown(UIMouseEvent evt, UIElement listeningElement)
		{
            var list = Parent.Parent as UIList;
            int i = list._items.IndexOf(this);

            if(i < list.Count - 1)
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

		public override void Click(UIMouseEvent evt)
        {
            if (Main.mouseItem.IsAir || storedHeight > 0) return;

            AddItem(Main.mouseItem);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var pos = GetDimensions().ToRectangle().TopLeft();
            var target = new Rectangle((int)pos.X, (int)pos.Y, (int)GetDimensions().Width, 32);
            ManualGeneratorMenu.DrawBox(spriteBatch, target, color);

            if (target.Contains(Main.MouseScreen.ToPoint()))
                Main.hoverItemName = rule.Tooltip + "\nLeft click this while holding an item to add it";

            if(removeButton.IsMouseHovering)
                Main.hoverItemName = "Remove rule";

            if (upButton.IsMouseHovering)
                Main.hoverItemName = "Move Up";

            if (downButton.IsMouseHovering)
                Main.hoverItemName = "Move Down";

            if(hideButton.IsMouseHovering)
                Main.hoverItemName = storedHeight > 0 ? "Show Items" : "Hide Items";

            Utils.DrawBorderString(spriteBatch, rule.Name, pos + new Vector2(32, 8), Color.White, 0.8f);

            if (storedHeight == 0)
                base.Draw(spriteBatch);
			else
			{
                removeButton.Draw(spriteBatch);
                upButton.Draw(spriteBatch);
                downButton.Draw(spriteBatch);
                hideButton.Draw(spriteBatch);
            }
        }

        //These handle adding/removing the elements and items from the appropriate lists, as well as re-sizing the element.
        public void AddItem(Item item)
        {
            var loot = rule.AddItem(item);

            var element = new LootElement(loot, rule.UsesWeight);
            lootElements.Add(element);
            lootElements.Height.Set(lootElements.Height.Pixels + element.Height.Pixels + 4, 0);
            Height.Set(Height.Pixels + element.Height.Pixels + 4, 0);
        }

        public void AddItem(Loot loot)
		{
            var element = new LootElement(loot, rule.UsesWeight);
            lootElements.Add(element);
            lootElements.Height.Set(lootElements.Height.Pixels + element.Height.Pixels + 4, 0);
            Height.Set(Height.Pixels + element.Height.Pixels + 4, 0);
        }
        
        public void RemoveItem(Loot loot, LootElement element)
        {
            rule.RemoveItem(loot);
            lootElements.Remove(element);
            lootElements.Height.Set(lootElements.Height.Pixels - element.Height.Pixels - 4, 0);
            Height.Set(Height.Pixels - element.Height.Pixels - 4, 0);
        }

        private void Remove(UIMouseEvent evt, UIElement listeningElement)
        {
            if (!(Parent.Parent.Parent is ChestCustomizerState)) return;

            ChestCustomizerState parent = Parent.Parent.Parent as ChestCustomizerState;
            parent.ruleElements.Remove(this);
        }
    }
}
